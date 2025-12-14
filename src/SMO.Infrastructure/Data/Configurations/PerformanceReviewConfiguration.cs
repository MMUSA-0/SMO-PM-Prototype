using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.EmployeePerformance;

namespace SMO.Infrastructure.Data.Configurations
{
    public class PerformanceReviewConfiguration : IEntityTypeConfiguration<PerformanceReview>
    {
        public void Configure(EntityTypeBuilder<PerformanceReview> builder)
        {
            // Table configuration
            builder.ToTable("PerformanceReviews", "Performance");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.ReviewType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.ReviewPeriod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Draft");

            builder.Property(r => r.OverallRating)
                .HasMaxLength(50);

            builder.Property(r => r.Recommendation)
                .HasMaxLength(100);

            builder.Property(r => r.Strengths)
                .HasMaxLength(4000);

            builder.Property(r => r.AreasForImprovement)
                .HasMaxLength(4000);

            builder.Property(r => r.DevelopmentPlan)
                .HasMaxLength(4000);

            builder.Property(r => r.EmployeeComments)
                .HasMaxLength(4000);

            builder.Property(r => r.ReviewerComments)
                .HasMaxLength(4000);

            builder.Property(r => r.OverallScore)
                .HasPrecision(5, 2);

            builder.Property(r => r.GoalAchievementScore)
                .HasPrecision(5, 2);

            builder.Property(r => r.CompetencyScore)
                .HasPrecision(5, 2);

            builder.Property(r => r.BehaviorScore)
                .HasPrecision(5, 2);

            builder.Property(r => r.InnovationScore)
                .HasPrecision(5, 2);

            builder.Property(r => r.RecommendedSalaryIncrease)
                .HasPrecision(5, 2);

            builder.Property(r => r.RecommendedBonus)
                .HasPrecision(18, 2);

            // Indexes
            builder.HasIndex(r => r.EmployeeId)
                .HasDatabaseName("IX_PerformanceReview_EmployeeId");

            builder.HasIndex(r => r.ReviewerId)
                .HasDatabaseName("IX_PerformanceReview_ReviewerId");

            builder.HasIndex(r => r.Status)
                .HasDatabaseName("IX_PerformanceReview_Status");

            builder.HasIndex(r => r.ReviewType)
                .HasDatabaseName("IX_PerformanceReview_ReviewType");

            builder.HasIndex(r => r.ReviewPeriod)
                .HasDatabaseName("IX_PerformanceReview_ReviewPeriod");

            builder.HasIndex(r => new { r.EmployeeId, r.ReviewPeriod })
                .IsUnique()
                .HasDatabaseName("IX_PerformanceReview_Employee_Period");

            builder.HasIndex(r => r.ReviewDate)
                .HasDatabaseName("IX_PerformanceReview_ReviewDate");

            // Relationships
            builder.HasOne(r => r.Employee)
                .WithMany(e => e.PerformanceReviews)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(r => r.PerformanceRatings)
                .WithOne(pr => pr.PerformanceReview)
                .HasForeignKey(pr => pr.PerformanceReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // Data seeding
            builder.HasData(
                new PerformanceReview
                {
                    Id = 1,
                    EmployeeId = 2,
                    ReviewerId = 1,
                    ReviewType = "Quarterly",
                    ReviewPeriod = "Q3-2024",
                    ReviewDate = new DateTime(2024, 10, 15),
                    PeriodStartDate = new DateTime(2024, 7, 1),
                    PeriodEndDate = new DateTime(2024, 9, 30),
                    Status = "Finalized",
                    OverallScore = 87.5m,
                    OverallRating = "Exceeds",
                    GoalAchievementScore = 85,
                    CompetencyScore = 90,
                    BehaviorScore = 88,
                    InnovationScore = 87,
                    Strengths = "Strong leadership skills, excellent project management, proactive problem solving",
                    AreasForImprovement = "Delegation skills, cross-functional collaboration",
                    DevelopmentPlan = "Leadership training program, cross-departmental project assignment",
                    Recommendation = "BonusIncrease",
                    RecommendedBonus = 15000,
                    SubmittedDate = new DateTime(2024, 10, 10),
                    ApprovedDate = new DateTime(2024, 10, 15),
                    ApprovedById = 1,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }
    }
}
