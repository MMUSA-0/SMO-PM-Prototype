using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Core;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for KPIValue entity
    /// Maps to the KPIValues table in the database
    /// </summary>
    public class KPIValueConfiguration : IEntityTypeConfiguration<KPIValue>
    {
        public void Configure(EntityTypeBuilder<KPIValue> builder)
        {
            // Table mapping
            builder.ToTable("KPIValues");

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
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.ActualValue)
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.ForecastValue)
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.AchievementRate)
                .HasColumnName("Achievement")
                .HasColumnType("decimal(5,2)");

            builder.Property(e => e.Status)
                .HasColumnName("DataQuality") // Map to existing column
                .HasMaxLength(50);

            builder.Property(e => e.PerformanceDriver)
                .HasColumnName("Comments") // Map to existing column
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.PerformanceBarrier)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.BriefExplanation)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DataEntrySource)
                .HasMaxLength(50);

            builder.Property(e => e.LastSyncDate);

            builder.Property(e => e.LastSyncSource)
                .HasMaxLength(50);

            // Audit fields (from AuditableEntity base class)
            builder.Property(e => e.CreatedOn)
                .HasColumnName("CreatedDate")
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(256);

            // Note: KPIValues table doesn't have UpdatedOn/UpdatedBy in the SQL schema
            builder.Ignore(e => e.UpdatedOn);
            builder.Ignore(e => e.UpdatedBy);
            builder.Ignore(e => e.IsDeleted);

            // Relationships
            builder.HasOne(e => e.KPI)
                .WithMany(k => k.KPIValues)
                .HasForeignKey(e => e.KPIId)
                .OnDelete(DeleteBehavior.Cascade);

            // Computed column for Period (maps to database Period column)
            builder.Property<DateTime>("Period")
                .HasComputedColumnSql("DATEFROMPARTS([Year], ISNULL([Month], 1), 1)");

            // Indexes
            builder.HasIndex(e => e.KPIId)
                .HasDatabaseName("IX_KPIValues_KPIId");

            builder.HasIndex(e => new { e.Year, e.Quarter, e.Month })
                .HasDatabaseName("IX_KPIValues_Period");

            // No global query filter since IsDeleted doesn't exist in this table
        }
    }
}
