using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// قيم مؤشرات أداء البرنامج - Program KPI Values
    /// </summary>
    public class ProgramKPIValue : AuditableEntity
    {
        public int ProgramPerformanceId { get; set; }
        public int KPIId { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? AchievementRate { get; set; }
        public string Status { get; set; } // Red, Yellow, Green
        public string PerformanceDriver { get; set; } // دوافع الأداء
        public string PerformanceBarrier { get; set; } // معوقات الأداء
        public string BriefExplanation { get; set; } // الشرح الموجز
        public bool IsAutomated { get; set; } // Calculated automatically or manually entered
        
        // Navigation Properties
        public virtual ProgramPerformance ProgramPerformance { get; set; }
        public virtual KPI KPI { get; set; }
    }
}
