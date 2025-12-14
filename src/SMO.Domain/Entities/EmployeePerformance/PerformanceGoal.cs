using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMO.Domain.Common;
using SMO.Domain.Entities.Shared;

namespace SMO.Domain.Entities.EmployeePerformance
{
    /// <summary>
    /// Performance Goal entity for Employee Performance Management
    /// Linked to Program KPIs and tracked through milestones
    /// </summary>
    public class PerformanceGoal : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(200)]
        public string TitleAr { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(2000)]
        public string DescriptionAr { get; set; }

        [Required]
        [StringLength(50)]
        public string GoalType { get; set; } // Strategic, Operational, Development, Innovation

        [Required]
        [StringLength(50)]
        public string Category { get; set; } // Business, Technical, Leadership, Personal

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Weight { get; set; } // Percentage weight in overall performance

        [Range(0, 100)]
        public decimal Progress { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "NotStarted"; // NotStarted, InProgress, Completed, Cancelled

        [StringLength(50)]
        public string Priority { get; set; } = "Medium"; // Low, Medium, High, Critical

        // Success Criteria
        [Required]
        [StringLength(2000)]
        public string SuccessCriteria { get; set; }

        [StringLength(50)]
        public string MeasurementUnit { get; set; } // Percentage, Count, Currency, etc.

        public decimal? TargetValue { get; set; }
        public decimal? ActualValue { get; set; }

        // Links to Vision 2030 Programs (optional)
        public int? ProgramKPIId { get; set; } // Link to program-level KPI

        // Review and Rating
        public DateTime? LastReviewDate { get; set; }
        
        [StringLength(50)]
        public string PerformanceLevel { get; set; } // Exceeds, Meets, BelowExpectations, NeedsImprovement

        // Navigation Properties
        public virtual ICollection<PerformanceGoalMilestone> Milestones { get; set; }
        
        public PerformanceGoal()
        {
            Milestones = new HashSet<PerformanceGoalMilestone>();
        }
    }
}
