using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMO.Application.Shared.Interfaces;
using SMO.Application.Shared.DTOs;
using SMO.Application.Features.Performance.DTOs;

namespace SMO.Application.Features.Performance.Services
{
    /// <summary>
    /// Performance Aggregator Service - إدارة الأداء
    /// Aggregates data from multiple modules for Performance Department (M1)
    /// Following Module Ownership Matrix - Performance owns Reports/Dashboards/Aggregation
    /// </summary>
    public interface IPerformanceAggregatorService
    {
        // Vision Level (UC-01, UC-02)
        Task<VisionLevelDashboardDto> GetVisionLevelDashboardAsync();
        Task<List<MacroeconomicIndicatorDto>> GetMEIPerformanceAsync();
        Task<List<StrategicObjectivePerformanceDto>> GetStrategicObjectivesPerformanceAsync();
        
        // Program Level (UC-03 to UC-13)
        Task<ProgramLevelDashboardDto> GetProgramLevelDashboardAsync(int? year = null, int? quarter = null);
        Task<ProgramPerformanceReportDto> GetProgramPerformanceDetailsAsync(int programId, int year, int quarter);
        Task<ProgramComparisonDto> CompareProgramsPerformanceAsync(List<int> programIds, int year, int quarter);
        
        // Initiative Level (UC-14 to UC-23)
        Task<InitiativeLevelDashboardDto> GetInitiativeLevelDashboardAsync(int programId);
        Task<InitiativePerformanceReportDto> GetInitiativePerformanceDetailsAsync(int initiativeId, int year, int quarter);
        
        // Executive Dashboard (NEW - Aggregated view)
        Task<ExecutiveDashboardDto> GetExecutiveDashboardAsync();
        Task<PerformanceAlertDto> GetPerformanceAlertsAsync();
        
        // Report Generation
        Task<byte[]> GenerateQuarterlyReportAsync(int programId, int year, int quarter, string format);
        Task<byte[]> GenerateExecutiveReportAsync(int year, int quarter, string format);
        Task<byte[]> ExportPerformanceDataAsync(string entityType, int entityId, string format);
    }

    public class PerformanceAggregatorService : IPerformanceAggregatorService
    {
        // Inject all shared services
        private readonly IKPIService _kpiService;
        private readonly IProgramService _programService;
        private readonly IRiskService _riskService;
        private readonly IBudgetService _budgetService;
        private readonly IInitiativeService _initiativeService;
        private readonly IMilestoneService _milestoneService;
        private readonly IPerformanceManagementService _performanceService; // Performance-specific data

        public PerformanceAggregatorService(
            IKPIService kpiService,
            IProgramService programService,
            IRiskService riskService,
            IBudgetService budgetService,
            IInitiativeService initiativeService,
            IMilestoneService milestoneService,
            IPerformanceManagementService performanceService)
        {
            _kpiService = kpiService;
            _programService = programService;
            _riskService = riskService;
            _budgetService = budgetService;
            _initiativeService = initiativeService;
            _milestoneService = milestoneService;
            _performanceService = performanceService;
        }

        public async Task<VisionLevelDashboardDto> GetVisionLevelDashboardAsync()
        {
            // Aggregate MEI performance
            var meiIndicators = await _performanceService.GetMacroeconomicIndicatorsAsync();
            
            // Get strategic objectives performance
            var objectives = await GetStrategicObjectivesPerformanceAsync();
            
            // Calculate overall Vision 2030 progress
            var overallProgress = CalculateVisionProgress(meiIndicators, objectives);
            
            return new VisionLevelDashboardDto
            {
                MEIIndicators = meiIndicators,
                StrategicObjectives = objectives,
                OverallVisionProgress = overallProgress,
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task<ProgramLevelDashboardDto> GetProgramLevelDashboardAsync(int? year = null, int? quarter = null)
        {
            year ??= DateTime.Now.Year;
            quarter ??= (DateTime.Now.Month - 1) / 3 + 1;

            // Get all programs from Program Management (M3)
            var programs = await _programService.GetActiveProgramsAsync();
            
            var programPerformances = new List<ProgramPerformanceCardDto>();
            
            foreach (var program in programs)
            {
                // Aggregate data from multiple sources
                var kpis = await _kpiService.GetKPIsByProgramAsync(program.Id);
                var initiatives = await _initiativeService.GetProgramInitiativesAsync(program.Id);
                var risks = await _riskService.GetProgramRisksAsync(program.Id);
                var budget = await _budgetService.GetProgramBudgetAsync(program.Id, year.Value);
                var achievements = await _performanceService.GetProgramAchievementsAsync(program.Id, year.Value, quarter.Value);
                var supportRequests = await _performanceService.GetProgramSupportRequestsAsync(program.Id);
                
                // Calculate performance metrics
                var kpiAchievement = CalculateKPIAchievement(kpis);
                var initiativeProgress = CalculateInitiativeProgress(initiatives);
                var overallPerformance = CalculateOverallPerformance(kpiAchievement, initiativeProgress, budget?.UtilizationRate ?? 0);
                
                programPerformances.Add(new ProgramPerformanceCardDto
                {
                    ProgramId = program.Id,
                    ProgramCode = program.Code,
                    ProgramNameAr = program.NameAr,
                    ProgramNameEn = program.NameEn,
                    Status = program.Status,
                    LogoUrl = program.LogoUrl,
                    
                    // Metrics
                    TotalKPIs = kpis.Count,
                    KPIsOnTrack = kpis.Count(k => k.Status == "Green"),
                    TotalInitiatives = initiatives.Count,
                    InitiativesCompleted = initiatives.Count(i => i.Status == "Completed"),
                    TotalAchievements = achievements.Count,
                    TotalRisks = risks.Count,
                    CriticalRisks = risks.Count(r => r.RiskLevel == "Critical"),
                    PendingSupportRequests = supportRequests.Count(s => s.Status == "Pending"),
                    BudgetUtilization = budget?.UtilizationRate ?? 0,
                    
                    // Performance
                    OverallPerformance = overallPerformance,
                    KPIAchievementRate = kpiAchievement,
                    InitiativeCompletionRate = initiativeProgress,
                    PerformanceStatus = DeterminePerformanceStatus(overallPerformance),
                    
                    // Report status
                    HasQuarterlyReport = await _performanceService.HasQuarterlyReportAsync(program.Id, year.Value, quarter.Value),
                    ReportStatus = await _performanceService.GetReportStatusAsync(program.Id, year.Value, quarter.Value)
                });
            }
            
            return new ProgramLevelDashboardDto
            {
                Year = year.Value,
                Quarter = quarter.Value,
                Programs = programPerformances,
                TotalPrograms = programs.Count,
                ProgramsOnTrack = programPerformances.Count(p => p.PerformanceStatus == "Green"),
                ProgramsAtRisk = programPerformances.Count(p => p.PerformanceStatus == "Yellow"),
                ProgramsOffTrack = programPerformances.Count(p => p.PerformanceStatus == "Red"),
                AveragePerformance = programPerformances.Average(p => p.OverallPerformance)
            };
        }

        public async Task<ExecutiveDashboardDto> GetExecutiveDashboardAsync()
        {
            // This is the new unified dashboard that aggregates everything
            var visionDashboard = await GetVisionLevelDashboardAsync();
            var programDashboard = await GetProgramLevelDashboardAsync();
            var alerts = await GetPerformanceAlertsAsync();
            
            // Get top performers and laggards
            var topPrograms = programDashboard.Programs
                .OrderByDescending(p => p.OverallPerformance)
                .Take(3)
                .ToList();
                
            var bottomPrograms = programDashboard.Programs
                .OrderBy(p => p.OverallPerformance)
                .Take(3)
                .ToList();
            
            return new ExecutiveDashboardDto
            {
                VisionProgress = visionDashboard.OverallVisionProgress,
                MEIPerformance = visionDashboard.MEIIndicators
                    .Average(m => m.PerformanceScore ?? 0),
                ProgramPerformance = programDashboard.AveragePerformance,
                TotalPrograms = programDashboard.TotalPrograms,
                TotalInitiatives = programDashboard.Programs.Sum(p => p.TotalInitiatives),
                TotalKPIs = programDashboard.Programs.Sum(p => p.TotalKPIs),
                
                TopPerformingPrograms = topPrograms,
                UnderperformingPrograms = bottomPrograms,
                
                CriticalAlerts = alerts.CriticalAlerts,
                WarningAlerts = alerts.WarningAlerts,
                InfoAlerts = alerts.InfoAlerts,
                
                LastUpdated = DateTime.UtcNow
            };
        }

        // Helper methods
        private decimal CalculateKPIAchievement(List<KPIDto> kpis)
        {
            if (!kpis.Any()) return 0;
            return kpis.Average(k => k.AchievementRate ?? 0);
        }

        private decimal CalculateInitiativeProgress(List<InitiativeDto> initiatives)
        {
            if (!initiatives.Any()) return 0;
            return initiatives.Average(i => i.ProgressPercentage);
        }

        private decimal CalculateOverallPerformance(decimal kpiAchievement, decimal initiativeProgress, decimal budgetUtilization)
        {
            // Weighted average: KPIs (40%), Initiatives (40%), Budget (20%)
            return (kpiAchievement * 0.4m) + (initiativeProgress * 0.4m) + (budgetUtilization * 0.2m);
        }

        private string DeterminePerformanceStatus(decimal performance)
        {
            return performance switch
            {
                >= 90 => "Green",
                >= 70 => "Yellow",
                _ => "Red"
            };
        }

        private decimal CalculateVisionProgress(List<MacroeconomicIndicatorDto> meiIndicators, List<StrategicObjectivePerformanceDto> objectives)
        {
            var meiProgress = meiIndicators.Any() ? meiIndicators.Average(m => m.PerformanceScore ?? 0) : 0;
            var objectiveProgress = objectives.Any() ? objectives.Average(o => o.AchievementRate) : 0;
            
            // Weighted average: MEI (60%), Strategic Objectives (40%)
            return (meiProgress * 0.6m) + (objectiveProgress * 0.4m);
        }

        // Implement remaining methods...
        public async Task<List<MacroeconomicIndicatorDto>> GetMEIPerformanceAsync()
        {
            return await _performanceService.GetMacroeconomicIndicatorsAsync();
        }

        public async Task<List<StrategicObjectivePerformanceDto>> GetStrategicObjectivesPerformanceAsync()
        {
            // TODO: Implement strategic objectives aggregation
            return new List<StrategicObjectivePerformanceDto>();
        }

        public async Task<ProgramPerformanceReportDto> GetProgramPerformanceDetailsAsync(int programId, int year, int quarter)
        {
            // TODO: Implement detailed report generation
            throw new NotImplementedException();
        }

        public async Task<ProgramComparisonDto> CompareProgramsPerformanceAsync(List<int> programIds, int year, int quarter)
        {
            // TODO: Implement program comparison
            throw new NotImplementedException();
        }

        public async Task<InitiativeLevelDashboardDto> GetInitiativeLevelDashboardAsync(int programId)
        {
            // TODO: Implement initiative dashboard
            throw new NotImplementedException();
        }

        public async Task<InitiativePerformanceReportDto> GetInitiativePerformanceDetailsAsync(int initiativeId, int year, int quarter)
        {
            // TODO: Implement initiative report
            throw new NotImplementedException();
        }

        public async Task<PerformanceAlertDto> GetPerformanceAlertsAsync()
        {
            // TODO: Implement performance alerts
            return new PerformanceAlertDto
            {
                CriticalAlerts = new List<AlertDto>(),
                WarningAlerts = new List<AlertDto>(),
                InfoAlerts = new List<AlertDto>()
            };
        }

        public async Task<byte[]> GenerateQuarterlyReportAsync(int programId, int year, int quarter, string format)
        {
            // TODO: Implement report generation
            throw new NotImplementedException();
        }

        public async Task<byte[]> GenerateExecutiveReportAsync(int year, int quarter, string format)
        {
            // TODO: Implement executive report
            throw new NotImplementedException();
        }

        public async Task<byte[]> ExportPerformanceDataAsync(string entityType, int entityId, string format)
        {
            // TODO: Implement data export
            throw new NotImplementedException();
        }
    }
}
