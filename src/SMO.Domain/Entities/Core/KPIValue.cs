using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Core
{
    /// <summary>
    /// KPI Value - قيمة مؤشر الأداء
    /// Bi-directional sync between M3 (Program Management) and M1 (Performance)
    /// </summary>
    public class KPIValue : AuditableEntity
    {
        public int KPIId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        public int? Month { get; set; }
        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? ForecastValue { get; set; }
        public decimal? AchievementRate { get; set; } // Auto-calculated
        public string Status { get; set; } // Red, Yellow, Green
        public string PerformanceDriver { get; set; } // دوافع الأداء
        public string PerformanceBarrier { get; set; } // معوقات الأداء
        public string BriefExplanation { get; set; } // الشرح الموجز
        public string DataEntrySource { get; set; } // Manual, Automated, Integration
        public DateTime? LastSyncDate { get; set; }
        public string LastSyncSource { get; set; } // M1-Performance, M3-Program, External
        
        // Navigation Properties
        public virtual KPI KPI { get; set; }
    }
}
