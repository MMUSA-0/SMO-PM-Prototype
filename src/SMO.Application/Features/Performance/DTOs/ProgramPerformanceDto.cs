using System;
using System.Collections.Generic;

namespace SMO.Application.Features.Performance.DTOs
{
    public class ProgramPerformanceDto
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string ProgramNameAr { get; set; }
        public string ProgramNameEn { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        public string ExecutiveSummaryAr { get; set; }
        public string ExecutiveSummaryEn { get; set; }
        public decimal? OverallPerformance { get; set; }
        public decimal? KPIAchievementRate { get; set; }
        public decimal? InitiativeCompletionRate { get; set; }
        public decimal? BudgetUtilizationRate { get; set; }
        public string Status { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovedBy { get; set; }
        public List<ProgramKPIValueDto> KPIValues { get; set; }
        public List<ProgramAchievementDto> Achievements { get; set; }
        public List<ProgramRiskDto> Risks { get; set; }
        public List<ProgramBudgetDto> Budgets { get; set; }
    }

    public class ProgramKPIValueDto
    {
        public int Id { get; set; }
        public int KPIId { get; set; }
        public string KPINameAr { get; set; }
        public string KPINameEn { get; set; }
        public string Unit { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? AchievementRate { get; set; }
        public string Status { get; set; }
        public string PerformanceDriver { get; set; }
        public string PerformanceBarrier { get; set; }
        public string BriefExplanation { get; set; }
    }

    public class ProgramAchievementDto
    {
        public int Id { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string ImpactLevel { get; set; }
        public string Category { get; set; }
        public DateTime AchievementDate { get; set; }
        public bool IsHighlighted { get; set; }
    }

    public class ProgramRiskDto
    {
        public int Id { get; set; }
        public string RiskCode { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Category { get; set; }
        public int Probability { get; set; }
        public int Impact { get; set; }
        public int RiskScore { get; set; }
        public string CurrentStatus { get; set; }
        public string MitigationPlanAr { get; set; }
        public string MitigationPlanEn { get; set; }
        public bool IsTopRisk { get; set; }
    }

    public class ProgramBudgetDto
    {
        public int Id { get; set; }
        public decimal ApprovedBudget { get; set; }
        public decimal ActualExpenditure { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal UtilizationRate { get; set; }
        public string Status { get; set; }
    }
}
