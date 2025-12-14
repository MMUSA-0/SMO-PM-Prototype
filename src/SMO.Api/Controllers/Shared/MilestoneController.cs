using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.Core;
using SMO.Domain.Entities.Performance;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace SMO.Api.Controllers.Shared
{
    /// <summary>
    /// SHARED Milestone Controller - Used by BOTH Performance Management and Vision 2030 Modules
    /// This controller handles milestones that can be linked to:
    /// - Performance Goals (Performance Module)
    /// - Strategic Initiatives (Vision 2030 Module)
    /// </summary>
    [ApiController]
    [Route("api/shared/[controller]")]
    public class MilestoneController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<MilestoneController> _logger;

        public MilestoneController(IUnitOfWork unitOfWork, ILogger<MilestoneController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all milestones with optional filtering by module
        /// </summary>
        /// <param name="module">Filter by module: Performance, Vision2030, or All</param>
        /// <param name="status">Filter by status</param>
        /// <param name="includeLinkedEntities">Include linked Performance Goals or Initiatives</param>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InitiativeMilestone>>> GetMilestones(
            [FromQuery] string module = "All",
            [FromQuery] string status = null,
            [FromQuery] bool includeLinkedEntities = false)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                
                // Build predicate based on module filter
                Expression<Func<InitiativeMilestone, bool>> predicate = m => true;
                
                if (!string.IsNullOrEmpty(status))
                {
                    predicate = m => m.Status == status;
                }

                // Add module-specific filtering based on LastSyncSource
                if (module != "All" && !string.IsNullOrEmpty(module))
                {
                    if (module == "Performance")
                    {
                        predicate = m => m.LastSyncSource == "M1-Performance" || m.LastSyncSource == null;
                    }
                    else if (module == "Vision2030")
                    {
                        predicate = m => m.LastSyncSource == "M3-Program" || m.LastSyncSource == null;
                    }
                }

                var includes = new List<Expression<Func<InitiativeMilestone, object>>>();
                
                if (includeLinkedEntities)
                {
                    includes.Add(m => m.Initiative);
                    includes.Add(m => m.SubMilestones);
                }

                var milestones = await repo.GetAsync(
                    predicate: predicate,
                    orderBy: q => q.OrderBy(m => m.PlannedDate).ThenBy(m => m.Code),
                    includes: includes,
                    disableTracking: true
                );

                _logger.LogInformation($"Retrieved {milestones.Count} milestones for module: {module}");
                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving milestones");
                return StatusCode(500, new { error = "An error occurred while retrieving milestones" });
            }
        }

        /// <summary>
        /// Get milestone by ID with all linked entities
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<InitiativeMilestone>> GetMilestone(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var milestones = await repo.GetAsync(
                    predicate: m => m.Id == id,
                    includes: new List<Expression<Func<InitiativeMilestone, object>>>
                    {
                        m => m.Initiative,
                        m => m.ParentMilestone,
                        m => m.SubMilestones
                    },
                    disableTracking: false
                );

                var milestone = milestones.FirstOrDefault();
                if (milestone == null)
                {
                    return NotFound(new { error = $"Milestone with ID {id} not found" });
                }

                // Get linked Performance Goals if any (would need PerformanceGoalMilestone entity)
                // This is a placeholder for when the linking table is created
                var linkedInfo = new
                {
                    milestone = milestone,
                    linkedTo = new
                    {
                        initiativeId = milestone.InitiativeId,
                        initiativeName = milestone.Initiative?.NameEn,
                        // performanceGoals = [] // Will be populated when linking table exists
                    }
                };

                return Ok(linkedInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving milestone {id}");
                return StatusCode(500, new { error = "An error occurred while retrieving the milestone" });
            }
        }

        /// <summary>
        /// Create new milestone (can be linked to Performance or Vision 2030)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<InitiativeMilestone>> CreateMilestone([FromBody] MilestoneCreateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var milestone = new InitiativeMilestone
                {
                    Code = dto.Code,
                    NameAr = dto.NameAr,
                    NameEn = dto.NameEn,
                    DescriptionAr = dto.DescriptionAr,
                    DescriptionEn = dto.DescriptionEn,
                    PlannedDate = dto.PlannedDate,
                    Status = dto.Status ?? "NotStarted",
                    ProgressPercentage = 0,
                    Owner = dto.Owner,
                    DeliverableType = dto.DeliverableType,
                    Budget = dto.Budget,
                    IsCriticalPath = dto.IsCriticalPath,
                    Notes = dto.Notes,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = User.Identity?.Name ?? "System",
                    LastSyncSource = dto.Module // "M1-Performance" or "M3-Program"
                };

                // Link to Initiative if provided (Vision 2030)
                if (dto.InitiativeId.HasValue)
                {
                    var initiativeRepo = _unitOfWork.Repository<Initiative>();
                    var initiativeExists = await initiativeRepo.GetFirstOrDefaultAsync(
                        predicate: i => i.Id == dto.InitiativeId.Value && i.IsDeleted == false
                    );
                    
                    if (initiativeExists == null)
                    {
                        return BadRequest(new { error = "Invalid Initiative ID" });
                    }
                    
                    milestone.InitiativeId = dto.InitiativeId.Value;
                }

                // Link to Performance Goals if provided (would need additional logic)
                if (dto.PerformanceGoalIds != null && dto.PerformanceGoalIds.Any())
                {
                    // TODO: Create PerformanceGoalMilestone linking records
                    _logger.LogInformation($"Will link milestone to {dto.PerformanceGoalIds.Count} performance goals");
                }

                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                await repo.InsertAsync(milestone, autoSave: false);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Created shared milestone: {milestone.NameEn} (ID: {milestone.Id}) for module: {dto.Module}");

                return CreatedAtAction(nameof(GetMilestone), new { id = milestone.Id }, milestone);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating milestone");
                return StatusCode(500, new { error = "An error occurred while creating the milestone" });
            }
        }

        /// <summary>
        /// Update existing milestone (updates reflect in BOTH modules)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMilestone(int id, [FromBody] MilestoneUpdateDto dto)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var existing = await repo.GetFirstOrDefaultAsync(
                    predicate: m => m.Id == id
                );

                if (existing == null)
                {
                    return NotFound(new { error = $"Milestone with ID {id} not found" });
                }

                // Update fields
                existing.Code = dto.Code ?? existing.Code;
                existing.NameAr = dto.NameAr ?? existing.NameAr;
                existing.NameEn = dto.NameEn ?? existing.NameEn;
                existing.DescriptionAr = dto.DescriptionAr ?? existing.DescriptionAr;
                existing.DescriptionEn = dto.DescriptionEn ?? existing.DescriptionEn;
                existing.PlannedDate = dto.PlannedDate ?? existing.PlannedDate;
                existing.ActualDate = dto.ActualDate ?? existing.ActualDate;
                existing.ForecastDate = dto.ForecastDate ?? existing.ForecastDate;
                existing.Status = dto.Status ?? existing.Status;
                existing.ProgressPercentage = dto.ProgressPercentage ?? existing.ProgressPercentage;
                existing.Owner = dto.Owner ?? existing.Owner;
                existing.DeliverableType = dto.DeliverableType ?? existing.DeliverableType;
                existing.Budget = dto.Budget ?? existing.Budget;
                existing.ActualCost = dto.ActualCost ?? existing.ActualCost;
                existing.IsCriticalPath = dto.IsCriticalPath ?? existing.IsCriticalPath;
                existing.Notes = dto.Notes ?? existing.Notes;
                existing.UpdatedOn = DateTime.UtcNow;
                existing.UpdatedBy = User.Identity?.Name ?? "System";
                existing.LastSyncDate = DateTime.UtcNow;

                // Auto-update status based on progress
                if (existing.ProgressPercentage == 100 && existing.Status != "Completed")
                {
                    existing.Status = "Completed";
                    existing.ActualDate = existing.ActualDate ?? DateTime.UtcNow;
                }

                repo.Update(existing);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Updated shared milestone: {existing.NameEn} (ID: {id}) - changes will reflect in BOTH modules");

                // TODO: Trigger notifications to both Performance and Vision 2030 modules
                await NotifyModulesOfMilestoneUpdate(existing);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating milestone {id}");
                return StatusCode(500, new { error = "An error occurred while updating the milestone" });
            }
        }

        /// <summary>
        /// Complete a milestone (affects both modules)
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<IActionResult> CompleteMilestone(int id, [FromBody] MilestoneCompletionDto dto)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var milestone = await repo.GetFirstOrDefaultAsync(
                    predicate: m => m.Id == id,
                    includes: new List<Expression<Func<InitiativeMilestone, object>>>
                    {
                        m => m.Initiative,
                        m => m.SubMilestones
                    }
                );

                if (milestone == null)
                {
                    return NotFound(new { error = $"Milestone with ID {id} not found" });
                }

                // Check if sub-milestones are completed
                if (milestone.SubMilestones.Any(sm => sm.Status != "Completed"))
                {
                    return BadRequest(new { error = "Cannot complete milestone with incomplete sub-milestones" });
                }

                milestone.Status = "Completed";
                milestone.ProgressPercentage = 100;
                milestone.ActualDate = dto.CompletionDate ?? DateTime.UtcNow;
                milestone.ActualCost = dto.ActualCost ?? milestone.ActualCost;
                milestone.Notes = dto.CompletionNotes ?? milestone.Notes;
                milestone.UpdatedOn = DateTime.UtcNow;
                milestone.UpdatedBy = User.Identity?.Name ?? "System";
                milestone.LastSyncDate = DateTime.UtcNow;

                repo.Update(milestone);

                // Update linked Initiative progress if applicable
                if (milestone.Initiative != null)
                {
                    await UpdateInitiativeProgress(milestone.InitiativeId);
                }

                // TODO: Update linked Performance Goals progress
                // await UpdatePerformanceGoalsProgress(milestone.Id);

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Completed milestone: {milestone.NameEn} (ID: {id}) - updating BOTH modules");

                return Ok(new { 
                    message = "Milestone completed successfully",
                    affectedModules = new[] { "Performance", "Vision2030" },
                    milestone = milestone
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error completing milestone {id}");
                return StatusCode(500, new { error = "An error occurred while completing the milestone" });
            }
        }

        /// <summary>
        /// Delete milestone (soft delete - affects both modules)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMilestone(int id)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var milestone = await repo.GetFirstOrDefaultAsync(
                    predicate: m => m.Id == id,
                    includes: new List<Expression<Func<InitiativeMilestone, object>>>
                    {
                        m => m.SubMilestones
                    }
                );

                if (milestone == null)
                {
                    return NotFound(new { error = $"Milestone with ID {id} not found" });
                }

                // Check for dependencies
                if (milestone.SubMilestones.Any())
                {
                    return BadRequest(new { error = "Cannot delete milestone with sub-milestones" });
                }

                // Mark as cancelled/deleted
                milestone.Status = "Cancelled";
                milestone.UpdatedOn = DateTime.UtcNow;
                milestone.UpdatedBy = User.Identity?.Name ?? "System";

                repo.Update(milestone);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation($"Deleted milestone: {milestone.NameEn} (ID: {id}) - removed from BOTH modules");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting milestone {id}");
                return StatusCode(500, new { error = "An error occurred while deleting the milestone" });
            }
        }

        /// <summary>
        /// Get milestones by Initiative ID (Vision 2030 specific)
        /// </summary>
        [HttpGet("initiative/{initiativeId}")]
        public async Task<ActionResult<IEnumerable<InitiativeMilestone>>> GetMilestonesByInitiative(int initiativeId)
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var milestones = await repo.GetAsync(
                    predicate: m => m.InitiativeId == initiativeId,
                    orderBy: q => q.OrderBy(m => m.PlannedDate).ThenBy(m => m.Code),
                    disableTracking: true
                );

                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving milestones for initiative {initiativeId}");
                return StatusCode(500, new { error = "An error occurred while retrieving milestones" });
            }
        }

        /// <summary>
        /// Get critical path milestones
        /// </summary>
        [HttpGet("critical-path")]
        public async Task<ActionResult<IEnumerable<InitiativeMilestone>>> GetCriticalPathMilestones()
        {
            try
            {
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var milestones = await repo.GetAsync(
                    predicate: m => m.IsCriticalPath && m.Status != "Completed" && m.Status != "Cancelled",
                    orderBy: q => q.OrderBy(m => m.PlannedDate),
                    includes: new List<Expression<Func<InitiativeMilestone, object>>>
                    {
                        m => m.Initiative
                    },
                    disableTracking: true
                );

                return Ok(milestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving critical path milestones");
                return StatusCode(500, new { error = "An error occurred while retrieving critical path milestones" });
            }
        }

        /// <summary>
        /// Get overdue milestones
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<InitiativeMilestone>>> GetOverdueMilestones()
        {
            try
            {
                var currentDate = DateTime.UtcNow;
                var repo = _unitOfWork.Repository<InitiativeMilestone>();
                var milestones = await repo.GetAsync(
                    predicate: m => m.PlannedDate < currentDate && 
                                   m.Status != "Completed" && 
                                   m.Status != "Cancelled",
                    orderBy: q => q.OrderByDescending(m => m.PlannedDate),
                    includes: new List<Expression<Func<InitiativeMilestone, object>>>
                    {
                        m => m.Initiative
                    },
                    disableTracking: true
                );

                var overdueMilestones = milestones.Select(m => new
                {
                    milestone = m,
                    daysOverdue = (currentDate - m.PlannedDate).Days,
                    affectsModules = GetAffectedModules(m)
                });

                return Ok(overdueMilestones);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving overdue milestones");
                return StatusCode(500, new { error = "An error occurred while retrieving overdue milestones" });
            }
        }

        #region Helper Methods

        private async Task UpdateInitiativeProgress(int initiativeId)
        {
            var initiativeRepo = _unitOfWork.Repository<Initiative>();
            var milestoneRepo = _unitOfWork.Repository<InitiativeMilestone>();

            var initiative = await initiativeRepo.GetFirstOrDefaultAsync(
                predicate: i => i.Id == initiativeId
            );

            if (initiative != null)
            {
                var milestones = await milestoneRepo.GetAsync(
                    predicate: m => m.InitiativeId == initiativeId
                );

                if (milestones.Any())
                {
                    var completedCount = milestones.Count(m => m.Status == "Completed");
                    var totalCount = milestones.Count;
                    
                    initiative.ProgressPercentage = (completedCount * 100) / totalCount;
                    initiative.UpdatedOn = DateTime.UtcNow;
                    
                    initiativeRepo.Update(initiative);
                }
            }
        }

        private async Task NotifyModulesOfMilestoneUpdate(InitiativeMilestone milestone)
        {
            // TODO: Implement notification logic
            // This would send notifications to both Performance Management and Vision 2030 modules
            _logger.LogInformation($"Notifying modules of milestone update: {milestone.Id}");
            
            // Example notification logic:
            // - Send notification to Performance Management if linked to performance goals
            // - Send notification to Vision 2030 if linked to initiatives
            // - Update audit logs for both modules
            // - Trigger any cascade updates
            
            await Task.CompletedTask;
        }

        private string[] GetAffectedModules(InitiativeMilestone milestone)
        {
            var modules = new List<string>();
            
            if (milestone.InitiativeId > 0)
            {
                modules.Add("Vision2030");
            }
            
            // TODO: Check for Performance Goal links when table exists
            // if (HasPerformanceGoalLinks(milestone.Id))
            // {
            //     modules.Add("Performance");
            // }
            
            if (!modules.Any())
            {
                modules.Add("Unlinked");
            }
            
            return modules.ToArray();
        }

        #endregion
    }

    #region DTOs

    public class MilestoneCreateDto
    {
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime PlannedDate { get; set; }
        public string Status { get; set; }
        public string Owner { get; set; }
        public string DeliverableType { get; set; }
        public decimal? Budget { get; set; }
        public bool IsCriticalPath { get; set; }
        public string Notes { get; set; }
        public string Module { get; set; } // "M1-Performance" or "M3-Program"
        public int? InitiativeId { get; set; } // For Vision 2030
        public List<int> PerformanceGoalIds { get; set; } // For Performance Management
    }

    public class MilestoneUpdateDto
    {
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public DateTime? ForecastDate { get; set; }
        public string Status { get; set; }
        public int? ProgressPercentage { get; set; }
        public string Owner { get; set; }
        public string DeliverableType { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public bool? IsCriticalPath { get; set; }
        public string Notes { get; set; }
    }

    public class MilestoneCompletionDto
    {
        public DateTime? CompletionDate { get; set; }
        public decimal? ActualCost { get; set; }
        public string CompletionNotes { get; set; }
    }

    #endregion
}
