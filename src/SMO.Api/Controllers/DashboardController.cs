using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.Core;
using SMO.Domain.Entities.EmployeePerformance;
using SMO.Domain.Entities.Risk;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMO.Api.Controllers
{
    /// <summary>
    /// Dashboard Controller - Provides aggregated data for all dashboard views
    /// Serves both Performance Management and Vision 2030 modules
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get main dashboard overview combining both modules
        /// </summary>
        [HttpGet("overview")]
        public async Task<ActionResult<object>> GetOverview()
        {
            try
            {
                // Vision 2030 Data
                var programs = await _unitOfWork.GetRepository<VisionProgram>()
                    .Query()
                    .Include(p => p.Initiatives)
                    .Include(p => p.KPIs)
                    .Where(p => p.IsActive)
                    .ToListAsync();

                // Employee Performance Data
                var employees = await _unitOfWork.GetRepository<Employee>()
                    .Query()
                    .Include(e => e.PerformanceGoals)
                    .Include(e => e.PerformanceReviews)
                    .Where(e => e.Status == "Active")
                    .ToListAsync();

                // Risk Data (Shared)
                var programRisks = await _unitOfWork.GetRepository<ProgramRisk>()
                    .Query()
                    .Where(r => r.Status == "Active")
                    .ToListAsync();

                var overview = new
                {
                    Timestamp = DateTime.UtcNow,
                    
                    Vision2030 = new
                    {
                        TotalPrograms = programs.Count,
                        ActivePrograms = programs.Count(p => p.Status == "Active"),
                        TotalInitiatives = programs.Sum(p => p.Initiatives.Count),
                        ActiveInitiatives = programs.Sum(p => p.Initiatives.Count(i => i.Status == "InProgress")),
                        TotalKPIs = programs.Sum(p => p.KPIs.Count),
                        AverageProgress = programs.Any() ? programs.Average(p => p.Progress ?? 0) : 0,
                        AtRiskPrograms = programs.Count(p => (p.Progress ?? 0) < 70)
                    },
                    
                    PerformanceManagement = new
                    {
                        TotalEmployees = employees.Count,
                        ActiveEmployees = employees.Count(e => e.Status == "Active"),
                        TotalGoals = employees.Sum(e => e.PerformanceGoals.Count),
                        ActiveGoals = employees.Sum(e => e.PerformanceGoals.Count(g => g.Status == "InProgress")),
                        CompletedGoals = employees.Sum(e => e.PerformanceGoals.Count(g => g.Status == "Completed")),
                        PendingReviews = employees.Count(e => !e.PerformanceReviews.Any(r => 
                            r.ReviewPeriod == $"Q{(DateTime.Now.Month - 1) / 3 + 1}-{DateTime.Now.Year}")),
                        AverageGoalProgress = employees.SelectMany(e => e.PerformanceGoals).Any()
                            ? employees.SelectMany(e => e.PerformanceGoals).Average(g => g.Progress)
                            : 0
                    },
                    
                    SharedResources = new
                    {
                        TotalRisks = programRisks.Count,
                        CriticalRisks = programRisks.Count(r => r.Severity == "Critical"),
                        HighRisks = programRisks.Count(r => r.Severity == "High"),
                        MediumRisks = programRisks.Count(r => r.Severity == "Medium"),
                        LowRisks = programRisks.Count(r => r.Severity == "Low"),
                        ActiveMitigations = programRisks.Count(r => r.MitigationStatus == "InProgress")
                    },
                    
                    QuickStats = new
                    {
                        Vision2030Progress = programs.Any() ? programs.Average(p => p.Progress ?? 0) : 0,
                        EmployeePerformance = employees.SelectMany(e => e.PerformanceReviews)
                            .Where(r => r.Status == "Finalized")
                            .Any() ? employees.SelectMany(e => e.PerformanceReviews)
                                .Where(r => r.Status == "Finalized")
                                .Average(r => r.OverallScore ?? 0) : 0,
                        OnTrackPercentage = CalculateOnTrackPercentage(programs, employees),
                        UpcomingMilestones = 15, // Should calculate from actual milestones
                        OverdueItems = 8 // Should calculate from actual data
                    },
                    
                    RecentActivity = new[]
                    {
                        new { Type = "Goal", Action = "Completed", Item = "Q4 Strategic Alignment", Time = "2 hours ago" },
                        new { Type = "Review", Action = "Submitted", Item = "Sara Al-Zahrani Q3 Review", Time = "5 hours ago" },
                        new { Type = "Initiative", Action = "Updated", Item = "Digital Transformation Phase 2", Time = "1 day ago" },
                        new { Type = "Risk", Action = "Mitigated", Item = "Resource Constraint Risk", Time = "2 days ago" }
                    },
                    
                    Notifications = new
                    {
                        Unread = 12,
                        Total = 45,
                        Priority = 3
                    }
                };

                return Ok(overview);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating dashboard overview");
                return StatusCode(500, new { error = "An error occurred while generating dashboard" });
            }
        }

        /// <summary>
        /// Get Vision 2030 specific dashboard
        /// </summary>
        [HttpGet("vision2030")]
        public async Task<ActionResult<object>> GetVision2030Dashboard()
        {
            try
            {
                var programs = await _unitOfWork.GetRepository<VisionProgram>()
                    .Query()
                    .Include(p => p.Initiatives)
                        .ThenInclude(i => i.Milestones)
                    .Include(p => p.KPIs)
                        .ThenInclude(k => k.KPIValues)
                    .Include(p => p.KPIs)
                        .ThenInclude(k => k.KPITargets)
                    .Where(p => p.IsActive)
                    .ToListAsync();

                var dashboard = new
                {
                    Summary = new
                    {
                        TotalPrograms = programs.Count,
                        ActivePrograms = programs.Count(p => p.Status == "Active"),
                        OnHoldPrograms = programs.Count(p => p.Status == "OnHold"),
                        CompletedPrograms = programs.Count(p => p.Status == "Completed")
                    },
                    
                    ProgramsBreakdown = programs.Select(p => new
                    {
                        p.Id,
                        p.Code,
                        p.Name,
                        p.NameAr,
                        p.Status,
                        p.Progress,
                        p.StartDate,
                        p.EndDate,
                        InitiativesCount = p.Initiatives.Count,
                        ActiveInitiatives = p.Initiatives.Count(i => i.Status == "InProgress"),
                        CompletedInitiatives = p.Initiatives.Count(i => i.Status == "Completed"),
                        KPIsCount = p.KPIs.Count,
                        KPIsOnTarget = p.KPIs.Count(k => IsKPIOnTarget(k)),
                        TotalMilestones = p.Initiatives.Sum(i => i.Milestones.Count),
                        CompletedMilestones = p.Initiatives.Sum(i => i.Milestones.Count(m => m.Status == "Completed")),
                        PerformanceColor = GetPerformanceColor(p.Progress ?? 0)
                    }).OrderByDescending(p => p.Progress),
                    
                    KPIOverview = new
                    {
                        Total = programs.Sum(p => p.KPIs.Count),
                        OnTarget = programs.Sum(p => p.KPIs.Count(k => IsKPIOnTarget(k))),
                        AtRisk = programs.Sum(p => p.KPIs.Count(k => IsKPIAtRisk(k))),
                        Behind = programs.Sum(p => p.KPIs.Count(k => IsKPIBehind(k))),
                        ByCategory = programs
                            .SelectMany(p => p.KPIs)
                            .GroupBy(k => k.Category ?? "Uncategorized")
                            .Select(g => new
                            {
                                Category = g.Key,
                                Count = g.Count(),
                                OnTarget = g.Count(k => IsKPIOnTarget(k))
                            })
                    },
                    
                    InitiativeStatus = new
                    {
                        Total = programs.Sum(p => p.Initiatives.Count),
                        NotStarted = programs.Sum(p => p.Initiatives.Count(i => i.Status == "NotStarted")),
                        InProgress = programs.Sum(p => p.Initiatives.Count(i => i.Status == "InProgress")),
                        Completed = programs.Sum(p => p.Initiatives.Count(i => i.Status == "Completed")),
                        Delayed = programs.Sum(p => p.Initiatives.Count(i => i.Status == "Delayed")),
                        Cancelled = programs.Sum(p => p.Initiatives.Count(i => i.Status == "Cancelled"))
                    },
                    
                    UpcomingMilestones = programs
                        .SelectMany(p => p.Initiatives)
                        .SelectMany(i => i.Milestones)
                        .Where(m => m.TargetDate > DateTime.Now && m.Status != "Completed")
                        .OrderBy(m => m.TargetDate)
                        .Take(10)
                        .Select(m => new
                        {
                            m.Title,
                            m.TargetDate,
                            DaysRemaining = (m.TargetDate - DateTime.Now).Days,
                            InitiativeName = m.Initiative?.Name,
                            ProgramName = m.Initiative?.Program?.Name
                        })
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Vision 2030 dashboard");
                return StatusCode(500, new { error = "An error occurred while generating dashboard" });
            }
        }

        /// <summary>
        /// Get Performance Management specific dashboard
        /// </summary>
        [HttpGet("performance")]
        public async Task<ActionResult<object>> GetPerformanceDashboard()
        {
            try
            {
                var currentQuarter = $"Q{(DateTime.Now.Month - 1) / 3 + 1}-{DateTime.Now.Year}";
                
                var employees = await _unitOfWork.GetRepository<Employee>()
                    .Query()
                    .Include(e => e.PerformanceGoals)
                        .ThenInclude(g => g.Milestones)
                    .Include(e => e.PerformanceReviews)
                        .ThenInclude(r => r.PerformanceRatings)
                    .Include(e => e.Manager)
                    .Where(e => e.Status == "Active")
                    .ToListAsync();

                var dashboard = new
                {
                    CurrentPeriod = currentQuarter,
                    
                    EmployeeOverview = new
                    {
                        Total = employees.Count,
                        Active = employees.Count(e => e.Status == "Active"),
                        ByDepartment = employees.GroupBy(e => e.Department)
                            .Select(g => new { Department = g.Key, Count = g.Count() }),
                        ByLevel = employees.GroupBy(e => e.Position)
                            .Select(g => new { Level = g.Key, Count = g.Count() })
                    },
                    
                    GoalsOverview = new
                    {
                        TotalGoals = employees.Sum(e => e.PerformanceGoals.Count),
                        ActiveGoals = employees.Sum(e => e.PerformanceGoals.Count(g => g.Status == "InProgress")),
                        CompletedGoals = employees.Sum(e => e.PerformanceGoals.Count(g => g.Status == "Completed")),
                        AverageProgress = employees.SelectMany(e => e.PerformanceGoals).Any()
                            ? employees.SelectMany(e => e.PerformanceGoals).Average(g => g.Progress)
                            : 0,
                        ByType = employees.SelectMany(e => e.PerformanceGoals)
                            .GroupBy(g => g.GoalType)
                            .Select(g => new { Type = g.Key, Count = g.Count() }),
                        ByCategory = employees.SelectMany(e => e.PerformanceGoals)
                            .GroupBy(g => g.Category)
                            .Select(g => new { Category = g.Key, Count = g.Count() })
                    },
                    
                    ReviewsStatus = new
                    {
                        CurrentQuarter = currentQuarter,
                        TotalReviews = employees.Sum(e => e.PerformanceReviews.Count(r => r.ReviewPeriod == currentQuarter)),
                        Completed = employees.Sum(e => e.PerformanceReviews.Count(r => 
                            r.ReviewPeriod == currentQuarter && r.Status == "Finalized")),
                        InProgress = employees.Sum(e => e.PerformanceReviews.Count(r => 
                            r.ReviewPeriod == currentQuarter && (r.Status == "Draft" || r.Status == "Submitted"))),
                        NotStarted = employees.Count(e => !e.PerformanceReviews.Any(r => r.ReviewPeriod == currentQuarter)),
                        AverageScore = employees.SelectMany(e => e.PerformanceReviews)
                            .Where(r => r.ReviewPeriod == currentQuarter && r.Status == "Finalized")
                            .Any() ? employees.SelectMany(e => e.PerformanceReviews)
                                .Where(r => r.ReviewPeriod == currentQuarter && r.Status == "Finalized")
                                .Average(r => r.OverallScore ?? 0) : 0
                    },
                    
                    TopPerformers = employees
                        .Where(e => e.PerformanceReviews.Any(r => r.Status == "Finalized"))
                        .Select(e => new
                        {
                            e.FullName,
                            e.Department,
                            e.Position,
                            LatestScore = e.PerformanceReviews
                                .Where(r => r.Status == "Finalized")
                                .OrderByDescending(r => r.ReviewDate)
                                .Select(r => r.OverallScore)
                                .FirstOrDefault() ?? 0,
                            GoalsCompleted = e.PerformanceGoals.Count(g => g.Status == "Completed")
                        })
                        .OrderByDescending(e => e.LatestScore)
                        .Take(10),
                    
                    UpcomingReviews = employees
                        .Where(e => !e.PerformanceReviews.Any(r => r.ReviewPeriod == currentQuarter))
                        .Select(e => new
                        {
                            e.FullName,
                            e.Department,
                            ManagerName = e.Manager?.FullName,
                            DaysOverdue = 0 // Calculate based on quarter end date
                        })
                        .Take(10),
                    
                    MilestoneTracking = new
                    {
                        Total = employees.SelectMany(e => e.PerformanceGoals)
                            .SelectMany(g => g.Milestones).Count(),
                        Completed = employees.SelectMany(e => e.PerformanceGoals)
                            .SelectMany(g => g.Milestones)
                            .Count(m => m.Status == "Completed"),
                        Overdue = employees.SelectMany(e => e.PerformanceGoals)
                            .SelectMany(g => g.Milestones)
                            .Count(m => m.TargetDate < DateTime.Now && m.Status != "Completed"),
                        UpcomingThisWeek = employees.SelectMany(e => e.PerformanceGoals)
                            .SelectMany(g => g.Milestones)
                            .Count(m => m.TargetDate >= DateTime.Now && 
                                       m.TargetDate <= DateTime.Now.AddDays(7) && 
                                       m.Status != "Completed")
                    }
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating performance dashboard");
                return StatusCode(500, new { error = "An error occurred while generating dashboard" });
            }
        }

        #region Helper Methods

        private decimal CalculateOnTrackPercentage(List<VisionProgram> programs, List<Employee> employees)
        {
            var visionOnTrack = programs.Count(p => (p.Progress ?? 0) >= 70);
            var performanceOnTrack = employees.Count(e => 
                e.PerformanceGoals.Any() && 
                e.PerformanceGoals.Average(g => g.Progress) >= 70);
            
            var total = programs.Count + employees.Count;
            var onTrack = visionOnTrack + performanceOnTrack;
            
            return total > 0 ? (decimal)(onTrack * 100.0 / total) : 0;
        }

        private bool IsKPIOnTarget(KPI kpi)
        {
            var currentValue = kpi.KPIValues.LastOrDefault()?.ActualValue ?? 0;
            var targetValue = kpi.KPITargets.LastOrDefault()?.TargetValue ?? 100;
            
            if (targetValue == 0) return false;
            
            var achievement = (currentValue / targetValue) * 100;
            return achievement >= 90;
        }

        private bool IsKPIAtRisk(KPI kpi)
        {
            var currentValue = kpi.KPIValues.LastOrDefault()?.ActualValue ?? 0;
            var targetValue = kpi.KPITargets.LastOrDefault()?.TargetValue ?? 100;
            
            if (targetValue == 0) return false;
            
            var achievement = (currentValue / targetValue) * 100;
            return achievement >= 70 && achievement < 90;
        }

        private bool IsKPIBehind(KPI kpi)
        {
            var currentValue = kpi.KPIValues.LastOrDefault()?.ActualValue ?? 0;
            var targetValue = kpi.KPITargets.LastOrDefault()?.TargetValue ?? 100;
            
            if (targetValue == 0) return true;
            
            var achievement = (currentValue / targetValue) * 100;
            return achievement < 70;
        }

        private string GetPerformanceColor(decimal progress)
        {
            if (progress >= 90) return "green";
            if (progress >= 70) return "yellow";
            if (progress >= 50) return "orange";
            return "red";
        }

        #endregion
    }
}
