using System;
using System.Collections.Generic;

namespace SMO.Application.Features.Performance.DTOs
{
    /// <summary>
    /// Dashboard DTOs for Performance Management Module
    /// These are aggregation DTOs that combine data from multiple modules
    /// </summary>
    
    public class VisionLevelDashboardDto
    {
        public List<MacroeconomicIndicatorDto> MEIIndicators { get; set; }
        public List<StrategicObjectivePerformanceDto> StrategicObjectives { get; set; }
        public decimal OverallVisionProgress { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class ProgramLevelDashboardDto
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public List<ProgramPerformanceCardDto> Programs { get; set; }
        public int TotalPrograms { get; set; }
        public int ProgramsOnTrack { get; set; }
        public int ProgramsAtRisk { get; set; }
        public int ProgramsOffTrack { get; set; }
        public decimal AveragePerformance { get; set; }
    }

    public class ProgramPerformanceCardDto
    {
        // Program info
        public int ProgramId { get; set; }
        public string ProgramCode { get; set; }
        public string ProgramNameAr { get; set; }
        public string ProgramNameEn { get; set; }
        public string Status { get; set; }
        public string LogoUrl { get; set; }
        
        // Metrics (aggregated from multiple modules)
        public int TotalKPIs { get; set; }
        public int KPIsOnTrack { get; set; }
        public int TotalInitiatives { get; set; }
        public int InitiativesCompleted { get; set; }
        public int TotalAchievements { get; set; }
        public int TotalRisks { get; set; }
        public int CriticalRisks { get; set; }
        public int PendingSupportRequests { get; set; }
        public decimal BudgetUtilization { get; set; }
        
        // Performance calculations
        public decimal OverallPerformance { get; set; }
        public decimal KPIAchievementRate { get; set; }
        public decimal InitiativeCompletionRate { get; set; }
        public string PerformanceStatus { get; set; } // Red, Yellow, Green
        
        // Report status
        public bool HasQuarterlyReport { get; set; }
        public string ReportStatus { get; set; } // Draft, Submitted, Approved
    }

    public class InitiativeLevelDashboardDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public List<InitiativePerformanceCardDto> Initiatives { get; set; }
        public int TotalInitiatives { get; set; }
        public int InitiativesOnTrack { get; set; }
        public int InitiativesDelayed { get; set; }
        public int InitiativesCompleted { get; set; }
        public decimal AverageProgress { get; set; }
    }

    public class InitiativePerformanceCardDto
    {
        public int InitiativeId { get; set; }
        public string InitiativeCode { get; set; }
        public string InitiativeNameAr { get; set; }
        public string InitiativeNameEn { get; set; }
        public string Status { get; set; }
        public int ProgressPercentage { get; set; }
        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
        public decimal BudgetUtilization { get; set; }
        public int DaysRemaining { get; set; }
        public string PerformanceStatus { get; set; }
    }

    public class ExecutiveDashboardDto
    {
        // High-level metrics
        public decimal VisionProgress { get; set; }
        public decimal MEIPerformance { get; set; }
        public decimal ProgramPerformance { get; set; }
        public int TotalPrograms { get; set; }
        public int TotalInitiatives { get; set; }
        public int TotalKPIs { get; set; }
        
        // Top/Bottom performers
        public List<ProgramPerformanceCardDto> TopPerformingPrograms { get; set; }
        public List<ProgramPerformanceCardDto> UnderperformingPrograms { get; set; }
        
        // Alerts
        public List<AlertDto> CriticalAlerts { get; set; }
        public List<AlertDto> WarningAlerts { get; set; }
        public List<AlertDto> InfoAlerts { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }

    public class StrategicObjectivePerformanceDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public int Level { get; set; }
        public decimal AchievementRate { get; set; }
        public string Status { get; set; }
        public int LinkedPrograms { get; set; }
        public int LinkedKPIs { get; set; }
    }

    public class ProgramPerformanceReportDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        
        // Executive summary
        public string ExecutiveSummaryAr { get; set; }
        public string ExecutiveSummaryEn { get; set; }
        public string FuturePlansAr { get; set; }
        public string FuturePlansEn { get; set; }
        
        // Performance sections
        public List<Shared.DTOs.KPIValueDto> KPIValues { get; set; }
        public List<ProgramAchievementDto> Achievements { get; set; }
        public List<Shared.DTOs.ProgramRiskSummaryDto> Risks { get; set; }
        public Shared.DTOs.ProgramBudgetSummaryDto Budget { get; set; }
        public List<ProgramSupportRequestDto> SupportRequests { get; set; }
        
        // Calculated metrics
        public decimal OverallPerformance { get; set; }
        public string Status { get; set; }
    }

    public class ProgramComparisonDto
    {
        public int Year { get; set; }
        public int Quarter { get; set; }
        public List<ProgramComparisonItemDto> Programs { get; set; }
        public ComparisonMetricsDto Metrics { get; set; }
    }

    public class ProgramComparisonItemDto
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public decimal OverallPerformance { get; set; }
        public decimal KPIAchievement { get; set; }
        public decimal InitiativeProgress { get; set; }
        public decimal BudgetUtilization { get; set; }
        public int RiskScore { get; set; }
    }

    public class ComparisonMetricsDto
    {
        public decimal AveragePerformance { get; set; }
        public decimal MaxPerformance { get; set; }
        public decimal MinPerformance { get; set; }
        public decimal StandardDeviation { get; set; }
    }

    public class InitiativePerformanceReportDto
    {
        public int InitiativeId { get; set; }
        public string InitiativeName { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        
        // Progress details
        public int ProgressPercentage { get; set; }
        public List<MilestoneDto> Milestones { get; set; }
        public List<Shared.DTOs.KPIValueDto> KPIValues { get; set; }
        
        // Financial
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
        public decimal CostVariance { get; set; }
        
        // Schedule
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ForecastEndDate { get; set; }
        public int ScheduleVarianceDays { get; set; }
    }

    public class MilestoneDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string Status { get; set; }
        public int CompletionPercentage { get; set; }
    }

    public class PerformanceAlertDto
    {
        public List<AlertDto> CriticalAlerts { get; set; }
        public List<AlertDto> WarningAlerts { get; set; }
        public List<AlertDto> InfoAlerts { get; set; }
    }

    public class AlertDto
    {
        public string Id { get; set; }
        public string Type { get; set; } // Critical, Warning, Info
        public string Category { get; set; } // KPI, Initiative, Risk, Budget
        public string MessageAr { get; set; }
        public string MessageEn { get; set; }
        public int EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
}
