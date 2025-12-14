using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Core;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for InitiativeMilestone entity
    /// Maps to the Milestones table in the database
    /// </summary>
    public class InitiativeMilestoneConfiguration : IEntityTypeConfiguration<InitiativeMilestone>
    {
        public void Configure(EntityTypeBuilder<InitiativeMilestone> builder)
        {
            // Table mapping
            builder.ToTable("Milestones");

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
                .HasColumnType("date");

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

            builder.Property(e => e.InitiativeId)
                .IsRequired();

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

            // Relationships
            builder.HasOne(e => e.Initiative)
                .WithMany(i => i.Milestones)
                .HasForeignKey(e => e.InitiativeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.InitiativeId)
                .HasDatabaseName("IX_Milestones_InitiativeId");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Milestones_Status");

            builder.HasIndex(e => e.PlannedDate)
                .HasDatabaseName("IX_Milestones_PlannedDate");

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
