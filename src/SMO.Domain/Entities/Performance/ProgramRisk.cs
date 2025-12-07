using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// مخاطر البرنامج - Program Risks
    /// </summary>
    public class ProgramRisk : AuditableEntity
    {
        public int ProgramPerformanceId { get; set; }
        public string RiskCode { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Category { get; set; } // Strategic, Operational, Financial, Technical
        public int Probability { get; set; } // 1-5
        public int Impact { get; set; } // 1-5
        public int RiskScore { get; set; } // Probability × Impact
        public string CurrentStatus { get; set; } // Open, Mitigating, Closed
        public string MitigationPlanAr { get; set; }
        public string MitigationPlanEn { get; set; }
        public string ResponsibleParty { get; set; }
        public DateTime? TargetResolutionDate { get; set; }
        public bool IsTopRisk { get; set; } // Top 5 critical risks
        
        // Navigation Properties
        public virtual ProgramPerformance ProgramPerformance { get; set; }
    }
}
