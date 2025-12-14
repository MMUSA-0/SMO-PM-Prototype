using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Shared;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for shared Milestone entity
    /// This is a shared resource that can track both performance goals and strategic initiatives
    /// </summary>
    public class MilestoneConfiguration : IEntityTypeConfiguration<Milestone>
    {
        public void Configure(EntityTypeBuilder<Milestone> builder)
        {
            // Table mapping - Uses a separate shared milestones table
            builder.ToTable("SharedMilestones");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Title)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.TitleAr)
                .HasMaxLength(500);

            builder.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DescriptionAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.PlannedDate)
                .HasColumnType("date")
                .IsRequired();

            builder.Property(e => e.ActualDate)
                .HasColumnType("date");

            builder.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending");

            builder.Property(e => e.Progress)
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(0);

            builder.Property(e => e.IsCritical)
                .HasDefaultValue(false);

            builder.Property(e => e.Dependencies)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.Owner)
                .HasMaxLength(256);

            builder.Property(e => e.OriginModule)
                .HasMaxLength(50);

            // Audit fields (from AuditableEntity base class)
            builder.Property(e => e.CreatedOn)
                .HasColumnName("CreatedDate")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(256);

            builder.Property(e => e.UpdatedOn)
                .HasColumnName("ModifiedDate");

            builder.Property(e => e.UpdatedBy)
                .HasColumnName("ModifiedBy")
                .HasMaxLength(256);

            builder.Property(e => e.IsDeleted)
                .HasColumnName("IsActive")
                .HasConversion(
                    v => !v,  // Convert IsDeleted to IsActive
                    v => !v)  // Convert IsActive to IsDeleted
                .HasDefaultValue(false);

            // Relationships - Links to both modules
            builder.HasMany(e => e.InitiativeMilestones)
                .WithOne()
                .HasForeignKey("MilestoneId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.PerformanceGoalMilestones)
                .WithOne(pgm => pgm.Milestone)
                .HasForeignKey(pgm => pgm.MilestoneId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_SharedMilestones_Status");

            builder.HasIndex(e => e.PlannedDate)
                .HasDatabaseName("IX_SharedMilestones_PlannedDate");

            builder.HasIndex(e => e.OriginModule)
                .HasDatabaseName("IX_SharedMilestones_OriginModule");

            builder.HasIndex(e => e.Owner)
                .HasDatabaseName("IX_SharedMilestones_Owner");

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
