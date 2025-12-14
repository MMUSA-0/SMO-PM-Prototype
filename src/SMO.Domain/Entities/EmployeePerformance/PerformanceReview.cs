using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMO.Domain.Common;

namespace SMO.Domain.Entities.EmployeePerformance
{
    /// <summary>
    /// Performance Review entity for periodic employee evaluations
    /// </summary>
    public class PerformanceReview : BaseEntity
    {
        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Required]
        public int ReviewerId { get; set; }
        [ForeignKey("ReviewerId")]
        public virtual Employee Reviewer { get; set; }

        [Required]
        [StringLength(50)]
        public string ReviewType { get; set; } // Quarterly, MidYear, Annual, Probation, Project

        [Required]
        [StringLength(50)]
        public string ReviewPeriod { get; set; } // Q1-2024, H1-2024, 2024, etc.

        [Required]
        public DateTime ReviewDate { get; set; }

        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Submitted, UnderReview, Approved, Disputed, Finalized

        // Overall Performance
        [Range(0, 100)]
        public decimal? OverallScore { get; set; }

        [StringLength(50)]
        public string OverallRating { get; set; } // Outstanding, Exceeds, Meets, BelowExpectations, Unsatisfactory

        // Category Scores
        [Range(0, 100)]
        public decimal? GoalAchievementScore { get; set; }

        [Range(0, 100)]
        public decimal? CompetencyScore { get; set; }

        [Range(0, 100)]
        public decimal? BehaviorScore { get; set; }

        [Range(0, 100)]
        public decimal? InnovationScore { get; set; }

        // Feedback
        [StringLength(4000)]
        public string Strengths { get; set; }

        [StringLength(4000)]
        public string AreasForImprovement { get; set; }

        [StringLength(4000)]
        public string DevelopmentPlan { get; set; }

        [StringLength(4000)]
        public string EmployeeComments { get; set; }

        [StringLength(4000)]
        public string ReviewerComments { get; set; }

        // Recommendations
        [StringLength(100)]
        public string Recommendation { get; set; } // Promotion, BonusIncrease, Training, PIP, NoAction

        public decimal? RecommendedSalaryIncrease { get; set; }
        public decimal? RecommendedBonus { get; set; }

        // Workflow
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int? ApprovedById { get; set; }

        // Navigation Properties
        public virtual ICollection<PerformanceRating> PerformanceRatings { get; set; }

        public PerformanceReview()
        {
            PerformanceRatings = new HashSet<PerformanceRating>();
        }
    }
}
