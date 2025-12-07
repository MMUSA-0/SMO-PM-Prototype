using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// قيم مؤشرات الاقتصاد الكلي - MEI Values
    /// </summary>
    public class MacroeconomicIndicatorValue : AuditableEntity
    {
        public int MacroeconomicIndicatorId { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; } // For quarterly measurements
        public int? Month { get; set; } // For monthly measurements
        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }
        public decimal? ForecastValue { get; set; }
        public decimal? PerformanceScore { get; set; } // Calculated automatically
        public string Description { get; set; }
        public string Status { get; set; } // Red, Yellow, Green
        public DateTime? LastCalculationDate { get; set; }
        public string CalculationSource { get; set; } // Manual, Formula
        
        // Navigation Properties
        public virtual MacroeconomicIndicator MacroeconomicIndicator { get; set; }
    }
}
