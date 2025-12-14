using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Shared
{
    /// <summary>
    /// Shared Milestone Entity - Used by both Performance Management and Vision 2030 modules
    /// This is a SHARED RESOURCE that tracks progress for both employee goals and strategic initiatives
    /// </summary>
    public class Milestone : AuditableEntity
    {
        public string Title { get; set; }
        public string TitleAr { get; set; }
        public string Description { get; set; }
        public string DescriptionAr { get; set; }
        
        // Dates
        public DateTime PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        
        // Status and Progress
        public string Status { get; set; } // Pending, InProgress, Completed, Delayed, Cancelled
        public decimal Progress { get; set; } // 0-100 percentage
        
        // Importance
        public bool IsCritical { get; set; }
        
        // Dependencies (JSON array of milestone IDs)
        public string Dependencies { get; set; }
        
        // Ownership
        public string Owner { get; set; }
        
        // Module Tracking
        public string OriginModule { get; set; } // Performance, Vision2030, Shared
        
        // Navigation Properties - Links to both modules
        public virtual ICollection<InitiativeMilestone> InitiativeMilestones { get; set; }
        public virtual ICollection<PerformanceGoalMilestone> PerformanceGoalMilestones { get; set; }
        
        public Milestone()
        {
            InitiativeMilestones = new HashSet<InitiativeMilestone>();
            PerformanceGoalMilestones = new HashSet<PerformanceGoalMilestone>();
            Status = "Pending";
            Progress = 0;
        }
        
        /// <summary>
        /// Marks milestone as complete and updates related entities
        /// </summary>
        public void MarkComplete()
        {
            Status = "Completed";
            Progress = 100;
            ActualDate = DateTime.UtcNow;
        }
        
        /// <summary>
        /// Checks if milestone is overdue
        /// </summary>
        public bool IsOverdue()
        {
            return Status != "Completed" && 
                   Status != "Cancelled" && 
                   PlannedDate < DateTime.UtcNow;
        }
    }
}
