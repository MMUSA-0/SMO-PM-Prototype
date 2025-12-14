using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.EmployeePerformance;
using SMO.Domain.Entities.Core;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMO.Api.Controllers.Performance
{
    /// <summary>
    /// CRITICAL: Employee Performance Management Controller
    /// Manages individual employee performance tracking aligned with Vision 2030 programs
    /// </summary>
    [ApiController]
    [Route("api/performance/[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PerformanceController> _logger;

        public PerformanceController(IUnitOfWork unitOfWork, ILogger<PerformanceController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Employee Management

        /// <summary>
        /// Get all employees with performance data
        /// </summary>
        [HttpGet("employees")]
        public async Task<ActionResult<IEnumerable<object>>> GetEmployees(
            [FromQuery] string department = null,
            [FromQuery] string status = "Active")
        {
            try
            {
                var query = _unitOfWork.GetRepository<Employee>()
                    .Query()
                    .Include(e => e.Manager)
                    .Include(e => e.PerformanceGoals)
                    .Include(e => e.PerformanceReviews)
                    .Where(e => e.Status == status);

                if (!string.IsNullOrEmpty(department))
                {
                    query = query.Where(e => e.Department == department);
                }

                var employees = await query
                    .Select(e => new
                    {
                        e.Id,
                        e.EmployeeCode,
                        e.FullName,
                        e.FullNameAr,
                        e.Department,
                        e.Position,
                        ManagerName = e.Manager != null ? e.Manager.FullName : null,
                        e.Status,
                        e.JoinDate,
                        ActiveGoals = e.PerformanceGoals.Count(g => g.Status == "InProgress"),
                        CompletedGoals = e.PerformanceGoals.Count(g => g.Status == "Completed"),
                        LastReviewDate = e.PerformanceReviews
                            .OrderByDescending(r => r.ReviewDate)
                            .Select(r => r.ReviewDate)
                            .FirstOrDefault(),
                        OverallRating = e.PerformanceReviews
                            .OrderByDescending(r => r.ReviewDate)
                            .Select(r => r.OverallRating)
                            .FirstOrDefault()
                    })
                    .ToListAsync();

                return Ok(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employees");
                return StatusCode(500, new { error = "An error occurred while retrieving employees" });
            }
        }

        /// <summary>
        /// Get employee details with full performance data
        /// </summary>
        [HttpGet("employees/{id}")]
        public async Task<ActionResult<object>> GetEmployee(int id)
        {
            try
            {
                var employee = await _unitOfWork.GetRepository<Employee>()
                    .Query()
                    .Include(e => e.Manager)
                    .Include(e => e.Subordinates)
                    .Include(e => e.PerformanceGoals)
                        .ThenInclude(g => g.Milestones)
                    .Include(e => e.PerformanceReviews)
                        .ThenInclude(r => r.PerformanceRatings)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                    return NotFound(new { error = $"Employee with ID {id} not found" });

                var result = new
                {
                    employee.Id,
                    employee.EmployeeCode,
                    employee.FullName,
                    employee.FullNameAr,
                    employee.Email,
                    employee.Department,
                    employee.Position,
                    employee.Status,
                    employee.JoinDate,
                    Manager = employee.Manager != null ? new { employee.Manager.Id, employee.Manager.FullName } : null,
                    Subordinates = employee.Subordinates.Select(s => new { s.Id, s.FullName, s.Position }),
                    
                    PerformanceGoals = employee.PerformanceGoals.Select(g => new
                    {
                        g.Id,
                        g.Title,
                        g.GoalType,
                        g.Category,
                        g.StartDate,
                        g.EndDate,
                        g.Weight,
                        g.Progress,
                        g.Status,
                        g.Priority,
                        g.TargetValue,
                        g.ActualValue,
                        g.PerformanceLevel,
                        MilestonesCount = g.Milestones.Count,
                        CompletedMilestones = g.Milestones.Count(m => m.Status == "Completed")
                    }),
                    
                    RecentReviews = employee.PerformanceReviews
                        .OrderByDescending(r => r.ReviewDate)
                        .Take(5)
                        .Select(r => new
                        {
                            r.Id,
                            r.ReviewType,
                            r.ReviewPeriod,
                            r.ReviewDate,
                            r.OverallScore,
                            r.OverallRating,
                            r.Status,
                            r.Recommendation,
                            ReviewerName = r.Reviewer.FullName
                        }),
                    
                    PerformanceMetrics = new
                    {
                        AverageGoalCompletion = employee.PerformanceGoals.Any() 
                            ? employee.PerformanceGoals.Average(g => g.Progress) 
                            : 0,
                        CurrentYearScore = employee.PerformanceReviews
                            .Where(r => r.ReviewDate.Year == DateTime.Now.Year)
                            .Average(r => r.OverallScore),
                        TotalGoals = employee.PerformanceGoals.Count,
                        CompletedGoals = employee.PerformanceGoals.Count(g => g.Status == "Completed"),
                        ActiveGoals = employee.PerformanceGoals.Count(g => g.Status == "InProgress")
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving employee {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving employee details" });
            }
        }

        /// <summary>
        /// Create new employee
        /// </summary>
        [HttpPost("employees")]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                employee.CreatedDate = DateTime.UtcNow;
                employee.CreatedBy = User.Identity?.Name ?? "System";

                await _unitOfWork.GetRepository<Employee>().AddAsync(employee);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Employee created: {EmployeeCode}", employee.EmployeeCode);
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating employee");
                return StatusCode(500, new { error = "An error occurred while creating employee" });
            }
        }

        #endregion

        #region Performance Dashboard

        /// <summary>
        /// Get performance dashboard data for the organization
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetPerformanceDashboard(
            [FromQuery] string department = null,
            [FromQuery] int year = 0)
        {
            try
            {
                if (year == 0) year = DateTime.Now.Year;

                var employeeQuery = _unitOfWork.GetRepository<Employee>()
                    .Query()
                    .Include(e => e.PerformanceGoals)
                    .Include(e => e.PerformanceReviews)
                    .Where(e => e.Status == "Active");

                if (!string.IsNullOrEmpty(department))
                {
                    employeeQuery = employeeQuery.Where(e => e.Department == department);
                }

                var employees = await employeeQuery.ToListAsync();

                var reviewsThisYear = employees
                    .SelectMany(e => e.PerformanceReviews)
                    .Where(r => r.ReviewDate.Year == year && r.Status == "Finalized");

                var dashboard = new
                {
                    Summary = new
                    {
                        TotalEmployees = employees.Count,
                        ActiveGoals = employees.Sum(e => e.PerformanceGoals.Count(g => g.Status == "InProgress")),
                        CompletedGoals = employees.Sum(e => e.PerformanceGoals.Count(g => g.Status == "Completed")),
                        ReviewsCompleted = reviewsThisYear.Count(),
                        AverageScore = reviewsThisYear.Any() ? reviewsThisYear.Average(r => r.OverallScore ?? 0) : 0
                    },
                    
                    RatingDistribution = reviewsThisYear
                        .GroupBy(r => r.OverallRating)
                        .Select(g => new { Rating = g.Key, Count = g.Count() }),
                    
                    DepartmentPerformance = employees
                        .GroupBy(e => e.Department)
                        .Select(g => new
                        {
                            Department = g.Key,
                            EmployeeCount = g.Count(),
                            AverageGoalProgress = g.SelectMany(e => e.PerformanceGoals).Any() 
                                ? g.SelectMany(e => e.PerformanceGoals).Average(goal => goal.Progress)
                                : 0,
                            CompletedGoals = g.Sum(e => e.PerformanceGoals.Count(goal => goal.Status == "Completed"))
                        }),
                    
                    TopPerformers = reviewsThisYear
                        .Where(r => r.OverallScore >= 90)
                        .OrderByDescending(r => r.OverallScore)
                        .Take(10)
                        .Select(r => new
                        {
                            EmployeeName = r.Employee.FullName,
                            Department = r.Employee.Department,
                            Score = r.OverallScore,
                            Rating = r.OverallRating
                        }),
                    
                    GoalCategories = employees
                        .SelectMany(e => e.PerformanceGoals)
                        .GroupBy(g => g.Category)
                        .Select(g => new
                        {
                            Category = g.Key,
                            Count = g.Count(),
                            AverageProgress = g.Average(goal => goal.Progress)
                        }),
                    
                    UpcomingReviews = employees
                        .Where(e => !e.PerformanceReviews.Any(r => 
                            r.ReviewPeriod == $"Q{(DateTime.Now.Month - 1) / 3 + 1}-{DateTime.Now.Year}"))
                        .Count(),
                    
                    Year = year,
                    Department = department ?? "All Departments",
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating performance dashboard");
                return StatusCode(500, new { error = "An error occurred while generating dashboard" });
            }
        }

        /// <summary>
        /// Get team performance summary for a manager
        /// </summary>
        [HttpGet("team-performance/{managerId}")]
        public async Task<ActionResult<object>> GetTeamPerformance(int managerId)
        {
            try
            {
                var team = await _unitOfWork.GetRepository<Employee>()
                    .Query()
                    .Include(e => e.PerformanceGoals)
                    .Include(e => e.PerformanceReviews)
                    .Where(e => e.ManagerId == managerId && e.Status == "Active")
                    .ToListAsync();

                if (!team.Any())
                    return NotFound(new { error = "No team members found for this manager" });

                var teamPerformance = new
                {
                    TeamSize = team.Count,
                    
                    TeamMembers = team.Select(e => new
                    {
                        e.Id,
                        e.FullName,
                        e.Position,
                        ActiveGoals = e.PerformanceGoals.Count(g => g.Status == "InProgress"),
                        GoalProgress = e.PerformanceGoals.Any() 
                            ? e.PerformanceGoals.Average(g => g.Progress)
                            : 0,
                        LastReview = e.PerformanceReviews
                            .OrderByDescending(r => r.ReviewDate)
                            .Select(r => new { r.ReviewDate, r.OverallRating })
                            .FirstOrDefault()
                    }),
                    
                    TeamMetrics = new
                    {
                        AverageGoalProgress = team.SelectMany(e => e.PerformanceGoals).Any()
                            ? team.SelectMany(e => e.PerformanceGoals).Average(g => g.Progress)
                            : 0,
                        TotalGoals = team.Sum(e => e.PerformanceGoals.Count),
                        CompletedGoals = team.Sum(e => e.PerformanceGoals.Count(g => g.Status == "Completed")),
                        PendingReviews = team.Count(e => !e.PerformanceReviews.Any(r => 
                            r.ReviewDate >= DateTime.Now.AddMonths(-3)))
                    }
                };

                return Ok(teamPerformance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving team performance for manager {ManagerId}", managerId);
                return StatusCode(500, new { error = "An error occurred while retrieving team performance" });
            }
        }

        #endregion
    }
}
