using System;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Performance
{
    /// <summary>
    /// إنجازات البرنامج - Program Achievements
    /// </summary>
    public class ProgramAchievement : AuditableEntity
    {
        public int ProgramPerformanceId { get; set; }
        public string TitleAr { get; set; }
        public string TitleEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public string ImpactLevel { get; set; } // High, Medium, Low
        public string Category { get; set; } // Strategic, Operational, Quick Win
        public DateTime AchievementDate { get; set; }
        public string LinkedObjectiveId { get; set; }
        public string SupportingDocuments { get; set; }
        public bool IsHighlighted { get; set; } // For dashboard display
        
        // Navigation Properties
        public virtual ProgramPerformance ProgramPerformance { get; set; }
    }
}
