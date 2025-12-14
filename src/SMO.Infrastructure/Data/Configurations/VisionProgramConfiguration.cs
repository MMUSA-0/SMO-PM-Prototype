using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Core;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for VisionProgram entity
    /// Maps to the Programs table in the database
    /// </summary>
    public class VisionProgramConfiguration : IEntityTypeConfiguration<VisionProgram>
    {
        public void Configure(EntityTypeBuilder<VisionProgram> builder)
        {
            // Table mapping
            builder.ToTable("Programs");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.NameAr)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.NameEn)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.DescriptionAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DescriptionEn)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.VisionAlignment)
                .HasMaxLength(1000);

            builder.Property(e => e.Status)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.ProgramOwner)
                .HasMaxLength(256);

            builder.Property(e => e.MinistryAr)
                .HasMaxLength(256);

            builder.Property(e => e.MinistryEn)
                .HasMaxLength(256);

            builder.Property(e => e.TotalBudget)
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.LogoUrl)
                .HasMaxLength(500);

            // Audit fields (from AuditableEntity base class)
            builder.Property(e => e.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.CreatedBy)
                .HasMaxLength(256);

            builder.Property(e => e.UpdatedOn);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(256);

            builder.Property(e => e.IsDeleted)
                .HasDefaultValue(false);

            // Relationships
            builder.HasMany(e => e.Initiatives)
                .WithOne(i => i.Program)
                .HasForeignKey(i => i.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.KPIs)
                .WithOne(k => k.Program)
                .HasForeignKey(k => k.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Risks)
                .WithOne()
                .HasForeignKey("ProgramId")
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_Programs_Code");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Programs_Status");

            builder.HasIndex(e => e.IsDeleted)
                .HasDatabaseName("IX_Programs_IsDeleted");

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}


