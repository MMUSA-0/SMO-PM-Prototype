using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.EmployeePerformance;

namespace SMO.Infrastructure.Data.Configurations
{
    public class PerformanceRatingConfiguration : IEntityTypeConfiguration<PerformanceRating>
    {
        public void Configure(EntityTypeBuilder<PerformanceRating> builder)
        {
            // Table configuration
            builder.ToTable("PerformanceRatings", "Performance");

            // Primary Key
            builder.HasKey(r => r.Id);

            // Properties
            builder.Property(r => r.RatingType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(r => r.RatingCategory)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Description)
                .HasMaxLength(500);

            builder.Property(r => r.Rating)
                .HasMaxLength(50);

            builder.Property(r => r.Comments)
                .HasMaxLength(2000);

            builder.Property(r => r.Evidence)
                .HasMaxLength(2000);

            builder.Property(r => r.Score)
                .HasPrecision(5, 2);

            builder.Property(r => r.Weight)
                .HasPrecision(5, 2)
                .HasDefaultValue(100);

            builder.Property(r => r.PreviousScore)
                .HasPrecision(5, 2);

            // Computed properties
            builder.Ignore(r => r.WeightedScore);
            builder.Ignore(r => r.Improvement);

            // Indexes
            builder.HasIndex(r => r.PerformanceReviewId)
                .HasDatabaseName("IX_PerformanceRating_ReviewId");

            builder.HasIndex(r => r.PerformanceGoalId)
                .HasDatabaseName("IX_PerformanceRating_GoalId");

            builder.HasIndex(r => r.EmployeeId)
                .HasDatabaseName("IX_PerformanceRating_EmployeeId");

            builder.HasIndex(r => r.RatingType)
                .HasDatabaseName("IX_PerformanceRating_Type");

            builder.HasIndex(r => new { r.PerformanceReviewId, r.RatingType })
                .HasDatabaseName("IX_PerformanceRating_Review_Type");

            // Relationships
            builder.HasOne(r => r.PerformanceReview)
                .WithMany(pr => pr.PerformanceRatings)
                .HasForeignKey(r => r.PerformanceReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.PerformanceGoal)
                .WithMany()
                .HasForeignKey(r => r.PerformanceGoalId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(r => r.Employee)
                .WithMany(e => e.PerformanceRatings)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Data seeding
            builder.HasData(
                new PerformanceRating
                {
                    Id = 1,
                    PerformanceReviewId = 1,
                    PerformanceGoalId = 1,
                    EmployeeId = 2,
                    RatingType = "Goal",
                    RatingCategory = "Vision 2030 Strategic Alignment",
                    Description = "Alignment of department objectives with Vision 2030",
                    Score = 85,
                    Rating = "Exceeds",
                    Weight = 30,
                    Comments = "Excellent progress on strategic alignment. 75% of KPIs successfully mapped.",
                    Evidence = "Quarterly reports, KPI dashboard, stakeholder feedback",
                    RatingDate = new DateTime(2024, 10, 15),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceRating
                {
                    Id = 2,
                    PerformanceReviewId = 1,
                    PerformanceGoalId = 2,
                    EmployeeId = 2,
                    RatingType = "Goal",
                    RatingCategory = "Team Performance Improvement",
                    Description = "Team productivity and achievement rates",
                    Score = 90,
                    Rating = "Exceeds",
                    Weight = 25,
                    Comments = "Team exceeded expectations with 85% target achievement rate.",
                    Evidence = "Team metrics dashboard, project completion reports",
                    RatingDate = new DateTime(2024, 10, 15),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceRating
                {
                    Id = 3,
                    PerformanceReviewId = 1,
                    EmployeeId = 2,
                    RatingType = "Competency",
                    RatingCategory = "Leadership",
                    Description = "Leadership and team management skills",
                    Score = 88,
                    Rating = "Exceeds",
                    Weight = 20,
                    Comments = "Strong leadership demonstrated during critical projects.",
                    Evidence = "360 feedback, team survey results",
                    RatingDate = new DateTime(2024, 10, 15),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceRating
                {
                    Id = 4,
                    PerformanceReviewId = 1,
                    EmployeeId = 2,
                    RatingType = "Behavior",
                    RatingCategory = "Collaboration",
                    Description = "Cross-functional collaboration and teamwork",
                    Score = 85,
                    Rating = "Meets",
                    Weight = 15,
                    Comments = "Good collaboration with most departments. Room for improvement with IT department.",
                    Evidence = "Peer feedback, project participation records",
                    RatingDate = new DateTime(2024, 10, 15),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceRating
                {
                    Id = 5,
                    PerformanceReviewId = 1,
                    EmployeeId = 2,
                    RatingType = "Initiative",
                    RatingCategory = "Innovation",
                    Description = "Innovation and process improvement initiatives",
                    Score = 87,
                    Rating = "Exceeds",
                    Weight = 10,
                    Comments = "Introduced new project tracking system that improved efficiency by 25%.",
                    Evidence = "Process improvement documentation, efficiency metrics",
                    RatingDate = new DateTime(2024, 10, 15),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }
    }
}
