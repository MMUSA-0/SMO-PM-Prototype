using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Core;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for KPI entity
    /// Maps to the KPIs table in the database
    /// </summary>
    public class KPIConfiguration : IEntityTypeConfiguration<KPI>
    {
        public void Configure(EntityTypeBuilder<KPI> builder)
        {
            // Table mapping
            builder.ToTable("KPIs");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.NameAr)
                .HasColumnName("NameAr")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.NameEn)
                .HasColumnName("Name")
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.DescriptionAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DescriptionEn)
                .HasColumnName("Description")
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.UnitOfMeasure)
                .HasColumnName("Unit")
                .HasMaxLength(50);

            builder.Property(e => e.Frequency)
                .HasMaxLength(50);

            builder.Property(e => e.BaselineValue)
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.TargetValue)
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.ActualValue)
                .HasColumnType("decimal(18,4)");

            builder.Property(e => e.Weight)
                .HasColumnType("decimal(5,2)");

            builder.Property(e => e.Category)
                .HasMaxLength(100);

            builder.Property(e => e.DataSource)
                .HasMaxLength(256);

            builder.Property(e => e.MeasurementMethod)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.Status)
                .HasMaxLength(50);

            builder.Property(e => e.Trend)
                .HasMaxLength(20);

            // Foreign keys
            builder.Property(e => e.ProgramId);
            builder.Property(e => e.InitiativeId);

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
            builder.HasOne(e => e.Program)
                .WithMany(p => p.KPIs)
                .HasForeignKey(e => e.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Initiative)
                .WithMany(i => i.KPIs)
                .HasForeignKey(e => e.InitiativeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.KPIValues)
                .WithOne(v => v.KPI)
                .HasForeignKey(v => v.KPIId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.KPITargets)
                .WithOne(t => t.KPI)
                .HasForeignKey(t => t.KPIId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_KPIs_Code");

            builder.HasIndex(e => e.InitiativeId)
                .HasDatabaseName("IX_KPIs_InitiativeId");

            builder.HasIndex(e => e.ProgramId)
                .HasDatabaseName("IX_KPIs_ProgramId");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_KPIs_Status");

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
