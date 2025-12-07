using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Financial
{
    /// <summary>
    /// Program Budget - ميزانية البرنامج
    /// Owned by M5 (Financial Management), Read-only for M1 (Performance)
    /// </summary>
    public class ProgramBudget : AuditableEntity
    {
        public int ProgramId { get; set; }
        public int FiscalYear { get; set; }
        public string BudgetCycle { get; set; } // Annual, Quarterly
        public decimal ApprovedBudget { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal ActualExpenditure { get; set; }
        public decimal EncumberedAmount { get; set; }
        public decimal RemainingBalance { get; set; }
        public decimal UtilizationRate { get; set; } // Auto-calculated: (ActualExpenditure / ApprovedBudget) × 100
        public string Currency { get; set; } // SAR, USD
        public string Status { get; set; } // OnTrack, AtRisk, OverBudget, UnderUtilized
        public string FundingSource { get; set; }
        public DateTime? LastReviewDate { get; set; }
        public string Notes { get; set; }
        
        // Sync metadata
        public DateTime? LastSyncDate { get; set; }
        public string SyncSource { get; set; } // M5-Finance
        
        // Navigation Properties
        public virtual Core.VisionProgram Program { get; set; }
    }
}
