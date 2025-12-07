using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Core
{
    /// <summary>
    /// KPI Target - هدف مؤشر الأداء
    /// Defines target values for KPIs across different time periods
    /// </summary>
    public class KPITarget : AuditableEntity
    {
        public int KPIId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public decimal TargetValue { get; set; }
        public decimal? StretchTarget { get; set; } // Ambitious target
        public decimal? MinimumAcceptable { get; set; } // Minimum threshold
        public string JustificationAr { get; set; }
        public string JustificationEn { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        
        // Navigation Properties
        public virtual KPI KPI { get; set; }
    }
}
