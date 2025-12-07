using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// ميزانية البرنامج - Program Budget
    /// </summary>
    public class ProgramBudget : AuditableEntity
    {
        public int ProgramPerformanceId { get; set; }
        public string BudgetYear { get; set; }
        public decimal ApprovedBudget { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal ActualExpenditure { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal UtilizationRate { get; set; }
        public string Currency { get; set; } // SAR, USD
        public string Status { get; set; } // OnTrack, AtRisk, OverBudget
        public string Notes { get; set; }
        public DateTime? LastSyncDate { get; set; }
        
        // Navigation Properties
        public virtual ProgramPerformance ProgramPerformance { get; set; }
    }
}
