using Framework.Core.Data;

namespace SMO.Domain.Entities.Shared
{
    /// <summary>
    /// Linking entity between Performance Goals and shared Milestones
    /// Enables milestones to track both strategic initiatives and employee goals
    /// </summary>
    public class PerformanceGoalMilestone : AuditableEntity
    {
        public int MilestoneId { get; set; }
        public int PerformanceGoalId { get; set; }
        public int EmployeeId { get; set; }
        
        // Contribution tracking
        public decimal ContributionPercentage { get; set; } // How much this goal contributes to the milestone
        public string Role { get; set; } // Primary, Supporting, Observer
        
        // Navigation Properties
        public virtual Milestone Milestone { get; set; }
        // PerformanceGoal will be added when entity is created
    }
}
