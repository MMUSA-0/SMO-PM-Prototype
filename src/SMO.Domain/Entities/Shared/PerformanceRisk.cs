using Framework.Core.Data;

namespace SMO.Domain.Entities.Shared
{
    /// <summary>
    /// Linking entity between Performance Management items and shared Risks
    /// Enables risks to be associated with performance goals, evaluations, etc.
    /// </summary>
    public class PerformanceRisk : AuditableEntity
    {
        public int RiskId { get; set; }
        public int PerformanceEntityId { get; set; }
        public string PerformanceEntityType { get; set; } // "Goal", "Evaluation", "Review", "DevelopmentPlan"
        
        // Additional relationship metadata
        public string RelationshipType { get; set; } // "Threatens", "Mitigates", "Monitors"
        public string Notes { get; set; }
        
        // Navigation Properties
        public virtual Risk Risk { get; set; }
    }
}
