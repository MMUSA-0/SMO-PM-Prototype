using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// طلبات الدعم للبرنامج - Program Support Requests
    /// </summary>
    public class ProgramSupportRequest : AuditableEntity
    {
        public int ProgramPerformanceId { get; set; }
        public string RequestCode { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string RequestType { get; set; } // Financial, Technical, Administrative, Legal
        public string Priority { get; set; } // Critical, High, Medium, Low
        public string RequestedFrom { get; set; } // Entity/Department name
        public DateTime RequestDate { get; set; }
        public DateTime? RequiredByDate { get; set; }
        public string Status { get; set; } // Pending, InProgress, Resolved, Rejected
        public string ResponseAr { get; set; }
        public string ResponseEn { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string RespondedBy { get; set; }
        
        // Navigation Properties
        public virtual ProgramPerformance ProgramPerformance { get; set; }
    }
}
