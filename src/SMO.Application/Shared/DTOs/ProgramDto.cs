using System;
using System.Collections.Generic;

namespace SMO.Application.Shared.DTOs
{
    /// <summary>
    /// Shared Program DTO - Used across all modules
    /// </summary>
    public class ProgramDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string VisionAlignment { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public string ProgramOwner { get; set; }
        public string MinistryAr { get; set; }
        public string MinistryEn { get; set; }
        public decimal? TotalBudget { get; set; }
        public string LogoUrl { get; set; }
        
        // Related counts
        public int TotalKPIs { get; set; }
        public int ActiveKPIs { get; set; }
        public int TotalInitiatives { get; set; }
        public int CompletedInitiatives { get; set; }
        
        // Performance summary (if available)
        public decimal? OverallPerformance { get; set; }
        public string PerformanceStatus { get; set; }
        public DateTime? LastUpdated { get; set; }
    }

    public class InitiativeDto
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string Status { get; set; }
        public DateTime PlannedStartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public int ProgressPercentage { get; set; }
        public string Owner { get; set; }
        public string Priority { get; set; }
        
        // Milestone summary
        public int TotalMilestones { get; set; }
        public int CompletedMilestones { get; set; }
    }

    public class ProgramStatisticsDto
    {
        public int ProgramId { get; set; }
        public int TotalKPIs { get; set; }
        public int KPIsOnTrack { get; set; }
        public int KPIsAtRisk { get; set; }
        public int KPIsOffTrack { get; set; }
        public int TotalInitiatives { get; set; }
        public int InitiativesNotStarted { get; set; }
        public int InitiativesInProgress { get; set; }
        public int InitiativesCompleted { get; set; }
        public int InitiativesOnHold { get; set; }
        public int TotalMilestones { get; set; }
        public int MilestonesCompleted { get; set; }
        public decimal BudgetUtilization { get; set; }
        public int TotalRisks { get; set; }
        public int CriticalRisks { get; set; }
    }

    public class ProgramPerformanceSummaryDto
    {
        public int ProgramId { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public decimal OverallPerformance { get; set; }
        public decimal KPIAchievementRate { get; set; }
        public decimal InitiativeCompletionRate { get; set; }
        public decimal BudgetUtilizationRate { get; set; }
        public string Status { get; set; }
        public int TotalAchievements { get; set; }
        public int TopRisks { get; set; }
        public int PendingSupportRequests { get; set; }
    }

    public class ProgramBudgetSummaryDto
    {
        public int ProgramId { get; set; }
        public int FiscalYear { get; set; }
        public decimal ApprovedBudget { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal ActualExpenditure { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal UtilizationRate { get; set; }
        public string Status { get; set; }
    }

    public class ProgramRiskSummaryDto
    {
        public int Id { get; set; }
        public string RiskCode { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string Category { get; set; }
        public int RiskScore { get; set; }
        public string RiskLevel { get; set; }
        public string CurrentStatus { get; set; }
        public string Owner { get; set; }
    }
}
