using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Core
{
    /// <summary>
    /// Key Performance Indicator - مؤشر الأداء الرئيسي
    /// Core entity shared across modules (M3: Program Management owns definition, M1: Performance owns values)
    /// </summary>
    public class KPI : AuditableEntity
    {
        public string Code { get; set; } // KPI.001, PRG.KPI.001, INIT.KPI.001
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string Category { get; set; } // Strategic, Operational, Tactical
        public string Type { get; set; } // Leading, Lagging
        public string Unit { get; set; } // %, Number, Currency, Days, etc.
        public string MeasurementFrequency { get; set; } // Annual, Quarterly, Monthly
        public string CalculationMethod { get; set; }
        public string DataSource { get; set; }
        public string Owner { get; set; }
        public string Polarity { get; set; } // Increasing, Decreasing, Nominal
        public decimal? BaselineValue { get; set; }
        public DateTime? BaselineDate { get; set; }
        public decimal? Target2030 { get; set; }
        public bool IsActive { get; set; }
        
        // Relationships
        public int? ProgramId { get; set; }
        public int? InitiativeId { get; set; }
        public int? StrategicObjectiveId { get; set; }
        
        // Navigation Properties
        public virtual VisionProgram Program { get; set; }
        public virtual Initiative Initiative { get; set; }
        public virtual ICollection<KPIValue> Values { get; set; }
        public virtual ICollection<KPITarget> Targets { get; set; }
        
        public KPI()
        {
            Values = new HashSet<KPIValue>();
            Targets = new HashSet<KPITarget>();
            IsActive = true;
        }
    }
}
