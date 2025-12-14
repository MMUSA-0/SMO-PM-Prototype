using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Core;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for KPITarget entity
    /// Maps to a KPITargets table (needs to be added to database)
    /// </summary>
    public class KPITargetConfiguration : IEntityTypeConfiguration<KPITarget>
    {
        public void Configure(EntityTypeBuilder<KPITarget> builder)
        {
            // Table mapping
            builder.ToTable("KPITargets");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.KPIId)
                .IsRequired();

            builder.Property(e => e.Year)
                .IsRequired();

            builder.Property(e => e.Quarter);

            builder.Property(e => e.Month);

            builder.Property(e => e.TargetValue)
                .HasColumnType("decimal(18,4)")
                .IsRequired();

            builder.Property(e => e.StretchTarget)
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.MinimumAcceptable)
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.JustificationAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.JustificationEn)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.IsApproved)
                .HasDefaultValue(false);

            builder.Property(e => e.ApprovedBy)
                .HasMaxLength(256);

            builder.Property(e => e.ApprovalDate);

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
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(e => e.KPI)
                .WithMany(k => k.KPITargets)
                .HasForeignKey(e => e.KPIId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.KPIId)
                .HasDatabaseName("IX_KPITargets_KPIId");

            builder.HasIndex(e => new { e.Year, e.Quarter, e.Month })
                .HasDatabaseName("IX_KPITargets_Period");

            builder.HasIndex(e => e.IsApproved)
                .HasDatabaseName("IX_KPITargets_IsApproved");

            // Unique constraint for KPI-Period combination
            builder.HasIndex(e => new { e.KPIId, e.Year, e.Quarter, e.Month })
                .IsUnique()
                .HasDatabaseName("UX_KPITargets_KPI_Period");

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
