using System;
using System.Collections.Generic;
using Framework.Core.Data;

namespace SMO.Domain.Entities.Core
{
    /// <summary>
    /// Initiative Milestone - معلم المبادرة
    /// Core entity shared across modules (M3: Program Management owns this)
    /// </summary>
    public class InitiativeMilestone : AuditableEntity
    {
        public int InitiativeId { get; set; }
        public string Code { get; set; } // MS.001, MS.002, etc.
        public string NameAr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionAr { get; set; }
        public string DescriptionEn { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public DateTime? ForecastDate { get; set; }
        public int ProgressPercentage { get; set; }
        public string Status { get; set; } // NotStarted, InProgress, Completed, Delayed, Cancelled
        public string Owner { get; set; }
        public string DeliverableType { get; set; }
        public decimal? Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public bool IsCriticalPath { get; set; }
        public int? ParentMilestoneId { get; set; }
        public string Notes { get; set; }
        
        // Sync metadata
        public DateTime? LastSyncDate { get; set; }
        public string LastSyncSource { get; set; } // M1-Performance, M3-Program
        
        // Navigation Properties
        public virtual Initiative Initiative { get; set; }
        public virtual InitiativeMilestone ParentMilestone { get; set; }
        public virtual ICollection<InitiativeMilestone> SubMilestones { get; set; }
        
        public InitiativeMilestone()
        {
            SubMilestones = new HashSet<InitiativeMilestone>();
        }
    }
}
