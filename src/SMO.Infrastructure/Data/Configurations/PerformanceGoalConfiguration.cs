using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.EmployeePerformance;

namespace SMO.Infrastructure.Data.Configurations
{
    public class PerformanceGoalConfiguration : IEntityTypeConfiguration<PerformanceGoal>
    {
        public void Configure(EntityTypeBuilder<PerformanceGoal> builder)
        {
            // Table configuration
            builder.ToTable("PerformanceGoals", "Performance");

            // Primary Key
            builder.HasKey(g => g.Id);

            // Properties
            builder.Property(g => g.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(g => g.TitleAr)
                .HasMaxLength(200);

            builder.Property(g => g.Description)
                .HasMaxLength(2000);

            builder.Property(g => g.DescriptionAr)
                .HasMaxLength(2000);

            builder.Property(g => g.GoalType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.Status)
                .HasMaxLength(50)
                .HasDefaultValue("NotStarted");

            builder.Property(g => g.Priority)
                .HasMaxLength(50)
                .HasDefaultValue("Medium");

            builder.Property(g => g.SuccessCriteria)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(g => g.MeasurementUnit)
                .HasMaxLength(50);

            builder.Property(g => g.PerformanceLevel)
                .HasMaxLength(50);

            builder.Property(g => g.Weight)
                .HasPrecision(5, 2);

            builder.Property(g => g.Progress)
                .HasPrecision(5, 2);

            builder.Property(g => g.TargetValue)
                .HasPrecision(18, 4);

            builder.Property(g => g.ActualValue)
                .HasPrecision(18, 4);

            // Indexes
            builder.HasIndex(g => g.EmployeeId)
                .HasDatabaseName("IX_PerformanceGoal_EmployeeId");

            builder.HasIndex(g => g.Status)
                .HasDatabaseName("IX_PerformanceGoal_Status");

            builder.HasIndex(g => g.GoalType)
                .HasDatabaseName("IX_PerformanceGoal_GoalType");

            builder.HasIndex(g => g.Priority)
                .HasDatabaseName("IX_PerformanceGoal_Priority");

            builder.HasIndex(g => new { g.EmployeeId, g.Status })
                .HasDatabaseName("IX_PerformanceGoal_EmployeeId_Status");

            builder.HasIndex(g => new { g.StartDate, g.EndDate })
                .HasDatabaseName("IX_PerformanceGoal_DateRange");

            // Relationships
            builder.HasOne(g => g.Employee)
                .WithMany(e => e.PerformanceGoals)
                .HasForeignKey(g => g.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(g => g.Milestones)
                .WithOne(m => m.PerformanceGoal)
                .HasForeignKey(m => m.PerformanceGoalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Data seeding
            builder.HasData(
                new PerformanceGoal
                {
                    Id = 1,
                    EmployeeId = 2,
                    Title = "Complete Vision 2030 Strategic Alignment",
                    TitleAr = "استكمال المواءمة الاستراتيجية لرؤية 2030",
                    Description = "Align department objectives with Vision 2030 programs",
                    GoalType = "Strategic",
                    Category = "Business",
                    Priority = "High",
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2024, 12, 31),
                    Weight = 30,
                    Progress = 75,
                    Status = "InProgress",
                    SuccessCriteria = "All department KPIs aligned with Vision 2030 programs",
                    MeasurementUnit = "Percentage",
                    TargetValue = 100,
                    ActualValue = 75,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new PerformanceGoal
                {
                    Id = 2,
                    EmployeeId = 2,
                    Title = "Improve Team Performance Metrics",
                    TitleAr = "تحسين مؤشرات أداء الفريق",
                    Description = "Enhance team productivity and achievement rates",
                    GoalType = "Operational",
                    Category = "Leadership",
                    Priority = "High",
                    StartDate = new DateTime(2024, 1, 1),
                    EndDate = new DateTime(2024, 6, 30),
                    Weight = 25,
                    Progress = 85,
                    Status = "InProgress",
                    SuccessCriteria = "Team achieves 90% of quarterly targets",
                    MeasurementUnit = "Percentage",
                    TargetValue = 90,
                    ActualValue = 85,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            );
        }
    }
}
