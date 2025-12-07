using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// مساهمة البرنامج في مؤشرات الاقتصاد الكلي
    /// Program MEI Contribution
    /// </summary>
    public class ProgramMEIContribution : AuditableEntity
    {
        public int ProgramId { get; set; }
        public int MacroeconomicIndicatorId { get; set; }
        public decimal ContributionPercentage { get; set; }
        public string ContributionType { get; set; } // Direct, Indirect
        public string JustificationAr { get; set; }
        public string JustificationEn { get; set; }
        public decimal? ActualContribution { get; set; }
        public decimal? TargetContribution { get; set; }
        public int Year { get; set; }
        public int? Quarter { get; set; }
        
        // Navigation Properties
        public virtual VisionProgram Program { get; set; }
        public virtual MacroeconomicIndicator MacroeconomicIndicator { get; set; }
    }
}
