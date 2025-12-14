using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SMO.Domain.Common;

namespace SMO.Domain.Entities.EmployeePerformance
{
    /// <summary>
    /// Performance Rating entity for individual goal/competency ratings within a review
    /// </summary>
    public class PerformanceRating : BaseEntity
    {
        [Required]
        public int PerformanceReviewId { get; set; }
        [ForeignKey("PerformanceReviewId")]
        public virtual PerformanceReview PerformanceReview { get; set; }

        public int? PerformanceGoalId { get; set; }
        [ForeignKey("PerformanceGoalId")]
        public virtual PerformanceGoal PerformanceGoal { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        [Required]
        [StringLength(50)]
        public string RatingType { get; set; } // Goal, Competency, Behavior, Initiative

        [Required]
        [StringLength(200)]
        public string RatingCategory { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(0, 100)]
        public decimal Score { get; set; }

        [StringLength(50)]
        public string Rating { get; set; } // Outstanding, Exceeds, Meets, BelowExpectations, Unsatisfactory

        [Range(0, 100)]
        public decimal Weight { get; set; } = 100;

        public decimal WeightedScore 
        { 
            get { return (Score * Weight) / 100; }
        }

        [StringLength(2000)]
        public string Comments { get; set; }

        [StringLength(2000)]
        public string Evidence { get; set; }

        public DateTime RatingDate { get; set; }

        // For tracking improvements
        public decimal? PreviousScore { get; set; }
        public decimal? Improvement 
        { 
            get 
            { 
                if (PreviousScore.HasValue)
                    return Score - PreviousScore.Value;
                return null;
            }
        }
    }
}
