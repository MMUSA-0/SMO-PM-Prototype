using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// أداء البرامج - Program Performance
    /// </summary>
    public class ProgramPerformance : AuditableEntity
    {
        public int ProgramId { get; set; }
        public int Year { get; set; }
        public int Quarter { get; set; }
        
        // Executive Summary
        public string ExecutiveSummaryAr { get; set; }
        public string ExecutiveSummaryEn { get; set; }
        
        // Performance Overview
        public decimal? OverallPerformance { get; set; }
        public decimal? KPIAchievementRate { get; set; }
        public decimal? InitiativeCompletionRate { get; set; }
        public decimal? BudgetUtilizationRate { get; set; }
        
        // Status
        public string Status { get; set; } // Draft, UnderReview, Verified, Rejected
        public DateTime? SubmissionDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string ApprovedBy { get; set; }
        public string RejectionReason { get; set; }
        
        // Navigation Properties
        public virtual VisionProgram Program { get; set; }
        public virtual ICollection<ProgramKPIValue> KPIValues { get; set; }
        public virtual ICollection<ProgramAchievement> Achievements { get; set; }
        public virtual ICollection<ProgramRisk> Risks { get; set; }
        public virtual ICollection<ProgramSupportRequest> SupportRequests { get; set; }
        public virtual ICollection<ProgramBudget> Budgets { get; set; }
        
        public ProgramPerformance()
        {
            KPIValues = new HashSet<ProgramKPIValue>();
            Achievements = new HashSet<ProgramAchievement>();
            Risks = new HashSet<ProgramRisk>();
            SupportRequests = new HashSet<ProgramSupportRequest>();
            Budgets = new HashSet<ProgramBudget>();
        }
    }
}
