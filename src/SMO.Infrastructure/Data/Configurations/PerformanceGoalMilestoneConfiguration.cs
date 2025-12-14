using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Shared;

namespace SMO.Infrastructure.Data.Configurations
{
    public class PerformanceGoalMilestoneConfiguration : IEntityTypeConfiguration<PerformanceGoalMilestone>
    {
        public void Configure(EntityTypeBuilder<PerformanceGoalMilestone> builder)
        {
            // Table configuration
            builder.ToTable("PerformanceGoalMilestones", "Performance");

            // Primary Key
            builder.HasKey(m => m.Id);

            // Properties
            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .HasMaxLength(2000);

            builder.Property(m => m.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.Property(m => m.Notes)
                .HasMaxLength(2000);

            builder.Property(m => m.Progress)
                .HasPrecision(5, 2)
                .HasDefaultValue(0);

            // Indexes
            builder.HasIndex(m => m.PerformanceGoalId)
                .HasDatabaseName("IX_PerformanceGoalMilestone_GoalId");

            builder.HasIndex(m => m.Status)
                .HasDatabaseName("IX_PerformanceGoalMilestone_Status");

            builder.HasIndex(m => m.TargetDate)
                .HasDatabaseName("IX_PerformanceGoalMilestone_TargetDate");

            builder.HasIndex(m => new { m.PerformanceGoalId, m.Status })
                .HasDatabaseName("IX_PerformanceGoalMilestone_Goal_Status");

            // Relationships
            builder.HasOne(m => m.PerformanceGoal)
                .WithMany(g => g.Milestones)
                .HasForeignKey(m => m.PerformanceGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Data seeding
            builder.HasData(
                new PerformanceGoalMilestone
                {
                    Id = 1,
                    PerformanceGoalId = 1,
                    Title = "Complete KPI Mapping",
                    Description = "Map all department KPIs to Vision 2030 programs",
                    TargetDate = new DateTime(2024, 3, 31),
                    CompletedDate = new DateTime(2024, 3, 28),
                    Status = "Completed",
                    Progress = 100,
                    Notes = "All 45 KPIs successfully mapped",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceGoalMilestone
                {
                    Id = 2,
                    PerformanceGoalId = 1,
                    Title = "Stakeholder Alignment Sessions",
                    Description = "Conduct alignment sessions with all stakeholders",
                    TargetDate = new DateTime(2024, 6, 30),
                    CompletedDate = new DateTime(2024, 6, 25),
                    Status = "Completed",
                    Progress = 100,
                    Notes = "12 sessions completed with all key stakeholders",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceGoalMilestone
                {
                    Id = 3,
                    PerformanceGoalId = 1,
                    Title = "Implementation Roadmap",
                    Description = "Develop and approve implementation roadmap",
                    TargetDate = new DateTime(2024, 9, 30),
                    Status = "InProgress",
                    Progress = 75,
                    Notes = "Roadmap drafted, pending final approval",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceGoalMilestone
                {
                    Id = 4,
                    PerformanceGoalId = 1,
                    Title = "Final Implementation",
                    Description = "Complete full implementation of aligned objectives",
                    TargetDate = new DateTime(2024, 12, 31),
                    Status = "Pending",
                    Progress = 0,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceGoalMilestone
                {
                    Id = 5,
                    PerformanceGoalId = 2,
                    Title = "Team Assessment",
                    Description = "Conduct comprehensive team performance assessment",
                    TargetDate = new DateTime(2024, 2, 29),
                    CompletedDate = new DateTime(2024, 2, 28),
                    Status = "Completed",
                    Progress = 100,
                    Notes = "Assessment completed for all 15 team members",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceGoalMilestone
                {
                    Id = 6,
                    PerformanceGoalId = 2,
                    Title = "Performance Improvement Plans",
                    Description = "Develop individual performance improvement plans",
                    TargetDate = new DateTime(2024, 3, 31),
                    CompletedDate = new DateTime(2024, 3, 30),
                    Status = "Completed",
                    Progress = 100,
                    Notes = "PIPs created for 5 team members requiring support",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceGoalMilestone
                {
                    Id = 7,
                    PerformanceGoalId = 2,
                    Title = "Mid-Year Review",
                    Description = "Conduct mid-year performance review",
                    TargetDate = new DateTime(2024, 6, 30),
                    Status = "InProgress",
                    Progress = 85,
                    Notes = "13 of 15 reviews completed",
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }
    }
}
