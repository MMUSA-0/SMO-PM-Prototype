using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Entities.EmployeePerformance;
using SMO.Infrastructure.Data;
using SMO.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMO.Api.Controllers.Performance
{
    /// <summary>
    /// Performance Review Controller - Manages employee performance reviews
    /// </summary>
    [ApiController]
    [Route("api/performance/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IUnitOfWork unitOfWork, ILogger<ReviewController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Get all reviews with filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReviews(
            [FromQuery] int? employeeId = null,
            [FromQuery] int? reviewerId = null,
            [FromQuery] string reviewType = null,
            [FromQuery] string status = null,
            [FromQuery] string period = null)
        {
            try
            {
                var query = _unitOfWork.GetRepository<PerformanceReview>()
                    .Query()
                    .Include(r => r.Employee)
                    .Include(r => r.Reviewer)
                    .Include(r => r.PerformanceRatings)
                    .AsQueryable();

                if (employeeId.HasValue)
                    query = query.Where(r => r.EmployeeId == employeeId.Value);

                if (reviewerId.HasValue)
                    query = query.Where(r => r.ReviewerId == reviewerId.Value);

                if (!string.IsNullOrEmpty(reviewType))
                    query = query.Where(r => r.ReviewType == reviewType);

                if (!string.IsNullOrEmpty(status))
                    query = query.Where(r => r.Status == status);

                if (!string.IsNullOrEmpty(period))
                    query = query.Where(r => r.ReviewPeriod == period);

                var reviews = await query
                    .OrderByDescending(r => r.ReviewDate)
                    .Select(r => new
                    {
                        r.Id,
                        r.ReviewType,
                        r.ReviewPeriod,
                        r.ReviewDate,
                        r.Status,
                        r.OverallScore,
                        r.OverallRating,
                        r.Recommendation,
                        Employee = new
                        {
                            r.Employee.Id,
                            r.Employee.FullName,
                            r.Employee.Department,
                            r.Employee.Position
                        },
                        Reviewer = new
                        {
                            r.Reviewer.Id,
                            r.Reviewer.FullName
                        },
                        Scores = new
                        {
                            r.GoalAchievementScore,
                            r.CompetencyScore,
                            r.BehaviorScore,
                            r.InnovationScore
                        },
                        RatingsCount = r.PerformanceRatings.Count,
                        r.SubmittedDate,
                        r.ApprovedDate
                    })
                    .ToListAsync();

                return Ok(reviews);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reviews");
                return StatusCode(500, new { error = "An error occurred while retrieving reviews" });
            }
        }

        /// <summary>
        /// Get review details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReview(int id)
        {
            try
            {
                var review = await _unitOfWork.GetRepository<PerformanceReview>()
                    .Query()
                    .Include(r => r.Employee)
                        .ThenInclude(e => e.PerformanceGoals)
                    .Include(r => r.Reviewer)
                    .Include(r => r.PerformanceRatings)
                        .ThenInclude(pr => pr.PerformanceGoal)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (review == null)
                    return NotFound(new { error = $"Review with ID {id} not found" });

                var result = new
                {
                    review.Id,
                    review.ReviewType,
                    review.ReviewPeriod,
                    review.ReviewDate,
                    review.PeriodStartDate,
                    review.PeriodEndDate,
                    review.Status,
                    review.OverallScore,
                    review.OverallRating,
                    Employee = new
                    {
                        review.Employee.Id,
                        review.Employee.FullName,
                        review.Employee.FullNameAr,
                        review.Employee.Department,
                        review.Employee.Position,
                        review.Employee.Email
                    },
                    Reviewer = new
                    {
                        review.Reviewer.Id,
                        review.Reviewer.FullName,
                        review.Reviewer.Position
                    },
                    Scores = new
                    {
                        review.GoalAchievementScore,
                        review.CompetencyScore,
                        review.BehaviorScore,
                        review.InnovationScore
                    },
                    Feedback = new
                    {
                        review.Strengths,
                        review.AreasForImprovement,
                        review.DevelopmentPlan,
                        review.ReviewerComments,
                        review.EmployeeComments
                    },
                    Recommendations = new
                    {
                        review.Recommendation,
                        review.RecommendedSalaryIncrease,
                        review.RecommendedBonus
                    },
                    Ratings = review.PerformanceRatings.Select(pr => new
                    {
                        pr.Id,
                        pr.RatingType,
                        pr.RatingCategory,
                        pr.Description,
                        pr.Score,
                        pr.Rating,
                        pr.Weight,
                        WeightedScore = (pr.Score * pr.Weight) / 100,
                        pr.Comments,
                        pr.Evidence,
                        GoalTitle = pr.PerformanceGoal != null ? pr.PerformanceGoal.Title : null
                    }).OrderBy(pr => pr.RatingType).ThenBy(pr => pr.RatingCategory),
                    Workflow = new
                    {
                        review.SubmittedDate,
                        review.ApprovedDate,
                        review.ApprovedById
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving review {Id}", id);
                return StatusCode(500, new { error = "An error occurred while retrieving review details" });
            }
        }

        /// <summary>
        /// Create new performance review
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PerformanceReview>> CreateReview([FromBody] PerformanceReview review)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check for existing review in same period
                var existingReview = await _unitOfWork.GetRepository<PerformanceReview>()
                    .Query()
                    .FirstOrDefaultAsync(r => 
                        r.EmployeeId == review.EmployeeId && 
                        r.ReviewPeriod == review.ReviewPeriod);

                if (existingReview != null)
                    return BadRequest(new { error = "A review already exists for this employee in this period" });

                review.CreatedDate = DateTime.UtcNow;
                review.CreatedBy = User.Identity?.Name ?? "System";
                review.Status = "Draft";

                await _unitOfWork.GetRepository<PerformanceReview>().AddAsync(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review created for employee {EmployeeId}, period {Period}", 
                    review.EmployeeId, review.ReviewPeriod);

                return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
                return StatusCode(500, new { error = "An error occurred while creating review" });
            }
        }

        /// <summary>
        /// Update review
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateReview(int id, [FromBody] PerformanceReview updatedReview)
        {
            try
            {
                var review = await _unitOfWork.GetRepository<PerformanceReview>()
                    .GetByIdAsync(id);

                if (review == null)
                    return NotFound(new { error = $"Review with ID {id} not found" });

                if (review.Status == "Finalized")
                    return BadRequest(new { error = "Cannot update finalized review" });

                // Update fields
                review.ReviewDate = updatedReview.ReviewDate;
                review.OverallScore = updatedReview.OverallScore;
                review.OverallRating = updatedReview.OverallRating;
                review.GoalAchievementScore = updatedReview.GoalAchievementScore;
                review.CompetencyScore = updatedReview.CompetencyScore;
                review.BehaviorScore = updatedReview.BehaviorScore;
                review.InnovationScore = updatedReview.InnovationScore;
                review.Strengths = updatedReview.Strengths;
                review.AreasForImprovement = updatedReview.AreasForImprovement;
                review.DevelopmentPlan = updatedReview.DevelopmentPlan;
                review.ReviewerComments = updatedReview.ReviewerComments;
                review.EmployeeComments = updatedReview.EmployeeComments;
                review.Recommendation = updatedReview.Recommendation;
                review.RecommendedSalaryIncrease = updatedReview.RecommendedSalaryIncrease;
                review.RecommendedBonus = updatedReview.RecommendedBonus;
                review.Status = updatedReview.Status;
                review.ModifiedDate = DateTime.UtcNow;
                review.ModifiedBy = User.Identity?.Name ?? "System";

                await _unitOfWork.GetRepository<PerformanceReview>().UpdateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review {Id} updated. Status: {Status}", id, review.Status);
                return Ok(new { message = "Review updated successfully", review.Status });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review {Id}", id);
                return StatusCode(500, new { error = "An error occurred while updating review" });
            }
        }

        /// <summary>
        /// Submit review for approval
        /// </summary>
        [HttpPost("{id}/submit")]
        public async Task<ActionResult> SubmitReview(int id)
        {
            try
            {
                var review = await _unitOfWork.GetRepository<PerformanceReview>()
                    .Query()
                    .Include(r => r.PerformanceRatings)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (review == null)
                    return NotFound(new { error = $"Review with ID {id} not found" });

                if (review.Status != "Draft")
                    return BadRequest(new { error = "Only draft reviews can be submitted" });

                if (!review.PerformanceRatings.Any())
                    return BadRequest(new { error = "Review must have at least one rating before submission" });

                review.Status = "Submitted";
                review.SubmittedDate = DateTime.UtcNow;
                review.ModifiedDate = DateTime.UtcNow;
                review.ModifiedBy = User.Identity?.Name ?? "System";

                await _unitOfWork.GetRepository<PerformanceReview>().UpdateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review {Id} submitted for approval", id);
                return Ok(new { message = "Review submitted for approval successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting review {Id}", id);
                return StatusCode(500, new { error = "An error occurred while submitting review" });
            }
        }

        /// <summary>
        /// Approve review
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<ActionResult> ApproveReview(int id)
        {
            try
            {
                var review = await _unitOfWork.GetRepository<PerformanceReview>()
                    .GetByIdAsync(id);

                if (review == null)
                    return NotFound(new { error = $"Review with ID {id} not found" });

                if (review.Status != "Submitted" && review.Status != "UnderReview")
                    return BadRequest(new { error = "Only submitted reviews can be approved" });

                review.Status = "Approved";
                review.ApprovedDate = DateTime.UtcNow;
                review.ApprovedById = 1; // Should get from authenticated user
                review.ModifiedDate = DateTime.UtcNow;
                review.ModifiedBy = User.Identity?.Name ?? "System";

                await _unitOfWork.GetRepository<PerformanceReview>().UpdateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review {Id} approved", id);
                return Ok(new { message = "Review approved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving review {Id}", id);
                return StatusCode(500, new { error = "An error occurred while approving review" });
            }
        }

        /// <summary>
        /// Finalize review
        /// </summary>
        [HttpPost("{id}/finalize")]
        public async Task<ActionResult> FinalizeReview(int id)
        {
            try
            {
                var review = await _unitOfWork.GetRepository<PerformanceReview>()
                    .GetByIdAsync(id);

                if (review == null)
                    return NotFound(new { error = $"Review with ID {id} not found" });

                if (review.Status != "Approved")
                    return BadRequest(new { error = "Only approved reviews can be finalized" });

                review.Status = "Finalized";
                review.ModifiedDate = DateTime.UtcNow;
                review.ModifiedBy = User.Identity?.Name ?? "System";

                await _unitOfWork.GetRepository<PerformanceReview>().UpdateAsync(review);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Review {Id} finalized", id);
                return Ok(new { message = "Review finalized successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finalizing review {Id}", id);
                return StatusCode(500, new { error = "An error occurred while finalizing review" });
            }
        }
    }
}
