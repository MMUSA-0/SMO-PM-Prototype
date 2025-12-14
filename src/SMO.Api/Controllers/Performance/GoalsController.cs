using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.EmployeePerformance;
using SMO.Domain.Entities.Shared;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMO.Api.Controllers.Performance
{
    /// <summary>
    /// Performance Goals Controller - Manages employee performance goals
    /// Links individual goals to Vision 2030 program KPIs
    /// </summary>
    [ApiController]
    [Route("api/performance/[controller]")]
    public class GoalsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GoalsController> _logger;

        public GoalsController(IUnitOfWork unitOfWork, ILogger<GoalsController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all performance goals with filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetGoals(
            [FromQuery] int? employeeId = null,
            [FromQuery] string status = null,
            [FromQuery] string goalType = null,
            [FromQuery] int year = 0)
        {
            try
            {
                if (year == 0) year = DateTime.Now.Year;

                var query = _unitOfWork.GetRepository<PerformanceGoal>()
                    .Query()
                    .Include(g => g.Employee)
                    .Include(g => g.Milestones)
                    .Where(g => g.StartDate.Year == year);

                if (employeeId.HasValue)
                    query = query.Where(g => g.EmployeeId == employeeId.Value);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(g => g.Status == status);

                if (!string.IsNullOrEmpty(goalType))
                    query = query.Where(g => g.GoalType == goalType);

                var goals = await query
                    .Select(g => new
                    {
                        g.Id,
                        g.Title,
                        g.TitleAr,
                        g.GoalType,
                        g.Category,
                        g.Priority,
                        g.StartDate,
                        g.EndDate,
                        g.Weight,
                        g.Progress,
                        g.Status,
                        g.TargetValue,
                        g.ActualValue,
                        g.PerformanceLevel,
                        Employee = new { g.Employee.Id, g.Employee.FullName, g.Employee.Department },
                        MilestonesCount = g.Milestones.Count,
                        CompletedMilestones = g.Milestones.Count(m => m.Status == "Completed"),
                        DaysRemaining = (g.EndDate - DateTime.Now).Days
                    })
                    .OrderBy(g => g.Priority == "Critical" ? 0 : g.Priority == "High" ? 1 : g.Priority == "Medium" ? 2 : 3)
                    .ThenBy(g => g.EndDate)
                    .ToListAsync();

                return Ok(goals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goals");
                return StatusCode(500, new { error = "An error occurred while retrieving goals" });
            }
        }

        /// <summary>
        /// Get goal details with milestones
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetGoal(int id)
        {
            try
            {
                var goal = await _unitOfWork.GetRepository<PerformanceGoal>()
                    .Query()
                    .Include(g => g.Employee)
                    .Include(g => g.Milestones)
                    .FirstOrDefaultAsync(g => g.Id == id);

                if (goal == null)
                    return NotFound(new { error = $"Goal with ID {id} not found" });

                var result = new
                {
                    goal.Id,
                    goal.Title,
                    goal.TitleAr,
                    goal.Description,
                    goal.DescriptionAr,
                    goal.GoalType,
                    goal.Category,
                    goal.Priority,
                    goal.StartDate,
                    goal.EndDate,
                    goal.Weight,
                    goal.Progress,
                    goal.Status,
                    goal.SuccessCriteria,
                    goal.MeasurementUnit,
                    goal.TargetValue,
                    goal.ActualValue,
                    goal.PerformanceLevel,
                    goal.LastReviewDate,
                    goal.ProgramKPIId,
                    Employee = new
                    {
                        goal.Employee.Id,
                        goal.Employee.FullName,
                        goal.Employee.Department,
                        goal.Employee.Position
                    },
                    Milestones = goal.Milestones.Select(m => new
                    {
                        m.Id,
                        m.Title,
                        m.Description,
                        m.TargetDate,
                        m.CompletedDate,
                        m.Status,
                        m.Progress,
                        m.Notes
                    }).OrderBy(m => m.TargetDate),
                    Metrics = new
                    {
                        DaysRemaining = (goal.EndDate - DateTime.Now).Days,
                        IsOverdue = goal.EndDate < DateTime.Now && goal.Status != "Completed",
                        ProjectedCompletion = CalculateProjectedCompletion(goal),
                        PerformanceRating = CalculatePerformanceRating(goal)
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving goal {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving goal details" });
            }
        }

        /// <summary>
        /// Create new performance goal
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PerformanceGoal>> CreateGoal([FromBody] PerformanceGoal goal)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Validate employee exists
                var employee = await _unitOfWork.GetRepository<Employee>()
                    .GetByIdAsync(goal.EmployeeId);

                if (employee == null)
                    return BadRequest(new { error = "Invalid employee ID" });

                // Ensure total weight doesn't exceed 100%
                var existingGoalsWeight = await _unitOfWork.GetRepository<PerformanceGoal>()
                    .Query()
                    .Where(g => g.EmployeeId == goal.EmployeeId && 
                               g.Status != "Cancelled" &&
                               g.EndDate >= DateTime.Now)
                    .SumAsync(g => g.Weight);

                if (existingGoalsWeight + goal.Weight > 100)
                {
                    return BadRequest(new { error = $"Total goal weight would exceed 100%. Current weight: {existingGoalsWeight}%" });
                }

                goal.CreatedDate = DateTime.UtcNow;
                goal.CreatedBy = User.Identity?.Name ?? "System";
                goal.Status = "NotStarted";
                goal.Progress = 0;

                await _unitOfWork.GetRepository<PerformanceGoal>().AddAsync(goal);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Goal created for employee {EmployeeId}: {Title}", goal.EmployeeId, goal.Title);
                return CreatedAtAction(nameof(GetGoal), new { id = goal.Id }, goal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating goal");
                return StatusCode(500, new { error = "An error occurred while creating goal" });
            }
        }

        /// <summary>
        /// Update goal progress and status
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateGoal(int id, [FromBody] PerformanceGoal updatedGoal)
        {
            try
            {
                var goal = await _unitOfWork.GetRepository<PerformanceGoal>()
                    .GetByIdAsync(id);

                if (goal == null)
                    return NotFound(new { error = $"Goal with ID {id} not found" });

                // Update modifiable fields
                goal.Title = updatedGoal.Title;
                goal.TitleAr = updatedGoal.TitleAr;
                goal.Description = updatedGoal.Description;
                goal.DescriptionAr = updatedGoal.DescriptionAr;
                goal.Priority = updatedGoal.Priority;
                goal.EndDate = updatedGoal.EndDate;
                goal.Weight = updatedGoal.Weight;
                goal.Progress = updatedGoal.Progress;
                goal.Status = updatedGoal.Status;
                goal.SuccessCriteria = updatedGoal.SuccessCriteria;
                goal.TargetValue = updatedGoal.TargetValue;
                goal.ActualValue = updatedGoal.ActualValue;
                goal.PerformanceLevel = updatedGoal.PerformanceLevel;
                goal.LastReviewDate = DateTime.UtcNow;
                goal.ModifiedDate = DateTime.UtcNow;
                goal.ModifiedBy = User.Identity?.Name ?? "System";

                // Auto-update status based on progress
                if (goal.Progress >= 100)
                {
                    goal.Status = "Completed";
                    goal.PerformanceLevel = CalculatePerformanceLevel(goal);
                }
                else if (goal.Progress > 0)
                {
                    goal.Status = "InProgress";
                }

                await _unitOfWork.GetRepository<PerformanceGoal>().UpdateAsync(goal);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Goal {Id} updated. Progress: {Progress}%, Status: {Status}", 
                    id, goal.Progress, goal.Status);

                return Ok(new { message = "Goal updated successfully", goal.Progress, goal.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating goal {Id}", id);
                return StatusCode(500, new { error = "An error occurred while updating goal" });
            }
        }

        /// <summary>
        /// Create milestone for a goal
        /// </summary>
        [HttpPost("{goalId}/milestones")]
        public async Task<ActionResult<PerformanceGoalMilestone>> CreateMilestone(
            int goalId, 
            [FromBody] PerformanceGoalMilestone milestone)
        {
            try
            {
                var goal = await _unitOfWork.GetRepository<PerformanceGoal>()
                    .GetByIdAsync(goalId);

                if (goal == null)
                    return NotFound(new { error = $"Goal with ID {goalId} not found" });

                milestone.PerformanceGoalId = goalId;
                milestone.CreatedDate = DateTime.UtcNow;
                milestone.CreatedBy = User.Identity?.Name ?? "System";
                milestone.Status = "Pending";

                await _unitOfWork.GetRepository<PerformanceGoalMilestone>().AddAsync(milestone);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Milestone created for goal {GoalId}: {Title}", goalId, milestone.Title);
                return Ok(milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating milestone for goal {GoalId}", goalId);
                return StatusCode(500, new { error = "An error occurred while creating milestone" });
            }
        }

        /// <summary>
        /// Update milestone status
        /// </summary>
        [HttpPut("milestones/{id}")]
        public async Task<ActionResult> UpdateMilestone(int id, [FromBody] PerformanceGoalMilestone updatedMilestone)
        {
            try
            {
                var milestone = await _unitOfWork.GetRepository<PerformanceGoalMilestone>()
                    .GetByIdAsync(id);

                if (milestone == null)
                    return NotFound(new { error = $"Milestone with ID {id} not found" });

                milestone.Title = updatedMilestone.Title;
                milestone.Description = updatedMilestone.Description;
                milestone.Status = updatedMilestone.Status;
                milestone.Progress = updatedMilestone.Progress;
                milestone.Notes = updatedMilestone.Notes;
                milestone.ModifiedDate = DateTime.UtcNow;
                milestone.ModifiedBy = User.Identity?.Name ?? "System";

                if (milestone.Status == "Completed" && !milestone.CompletedDate.HasValue)
                {
                    milestone.CompletedDate = DateTime.UtcNow;
                }

                await _unitOfWork.GetRepository<PerformanceGoalMilestone>().UpdateAsync(milestone);

                // Update parent goal progress
                await UpdateGoalProgressFromMilestones(milestone.PerformanceGoalId);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Milestone {Id} updated. Status: {Status}", id, milestone.Status);
                return Ok(new { message = "Milestone updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating milestone {Id}", id);
                return StatusCode(500, new { error = "An error occurred while updating milestone" });
            }
        }

        #region Helper Methods

        private async Task UpdateGoalProgressFromMilestones(int goalId)
        {
            var goal = await _unitOfWork.GetRepository<PerformanceGoal>()
                .Query()
                .Include(g => g.Milestones)
                .FirstOrDefaultAsync(g => g.Id == goalId);

            if (goal != null && goal.Milestones.Any())
            {
                var completedMilestones = goal.Milestones.Count(m => m.Status == "Completed");
                var totalMilestones = goal.Milestones.Count;
                goal.Progress = (decimal)(completedMilestones * 100.0 / totalMilestones);
                
                await _unitOfWork.GetRepository<PerformanceGoal>().UpdateAsync(goal);
            }
        }

        private string CalculatePerformanceLevel(PerformanceGoal goal)
        {
            if (goal.ActualValue.HasValue && goal.TargetValue.HasValue)
            {
                var achievement = (goal.ActualValue.Value / goal.TargetValue.Value) * 100;
                
                if (achievement >= 110) return "Exceeds";
                if (achievement >= 95) return "Meets";
                if (achievement >= 70) return "BelowExpectations";
                return "NeedsImprovement";
            }
            
            return goal.Progress >= 100 ? "Meets" : "BelowExpectations";
        }

        private DateTime? CalculateProjectedCompletion(PerformanceGoal goal)
        {
            if (goal.Progress <= 0) return null;
            
            var elapsed = (DateTime.Now - goal.StartDate).Days;
            var totalDays = elapsed / (goal.Progress / 100);
            return goal.StartDate.AddDays((int)totalDays);
        }

        private string CalculatePerformanceRating(PerformanceGoal goal)
        {
            var daysRemaining = (goal.EndDate - DateTime.Now).Days;
            var expectedProgress = ((DateTime.Now - goal.StartDate).Days / 
                                  (double)(goal.EndDate - goal.StartDate).Days) * 100;

            if (goal.Progress >= expectedProgress + 10) return "Ahead of Schedule";
            if (goal.Progress >= expectedProgress - 5) return "On Track";
            if (goal.Progress >= expectedProgress - 15) return "Slightly Behind";
            return "At Risk";
        }

        #endregion
    }
}
