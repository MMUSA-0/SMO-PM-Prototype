using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Core;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for Initiative entity
    /// Maps to the Initiatives table in the database
    /// </summary>
    public class InitiativeConfiguration : IEntityTypeConfiguration<Initiative>
    {
        public void Configure(EntityTypeBuilder<Initiative> builder)
        {
            // Table mapping
            builder.ToTable("Initiatives");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.NameAr)
                .HasColumnName("TitleAr")  // Map to existing database column
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.NameEn)
                .HasColumnName("Title")  // Map to existing database column
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.DescriptionAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DescriptionEn)
                .HasColumnName("Description")
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.ProgramId)
                .IsRequired();

            builder.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Draft");

            builder.Property(e => e.Budget)
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.ActualCost)
                .HasColumnName("SpentBudget")
                .HasColumnType("decimal(18,2)");

            builder.Property(e => e.PlannedStartDate)
                .HasColumnName("StartDate")
                .HasColumnType("date");

            builder.Property(e => e.PlannedEndDate)
                .HasColumnName("EndDate")
                .HasColumnType("date");

            builder.Property(e => e.ActualStartDate)
                .HasColumnType("date");

            builder.Property(e => e.ActualEndDate)
                .HasColumnType("date");

            builder.Property(e => e.ProgressPercentage)
                .HasColumnName("Progress")
                .HasDefaultValue(0);

            builder.Property(e => e.Priority)
                .HasMaxLength(20);

            builder.Property(e => e.Owner)
                .HasMaxLength(256);

            builder.Property(e => e.Scope)
                .HasColumnName("Type")
                .HasMaxLength(50);

            builder.Property(e => e.ExpectedOutcome)
                .HasColumnName("Category")
                .HasMaxLength(100);

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
                .WithMany(p => p.Initiatives)
                .HasForeignKey(e => e.ProgramId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Milestones)
                .WithOne(m => m.Initiative)
                .HasForeignKey(m => m.InitiativeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.KPIs)
                .WithOne(k => k.Initiative)
                .HasForeignKey(k => k.InitiativeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Risks)
                .WithOne()
                .HasForeignKey("InitiativeId")
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_Initiatives_Code");

            builder.HasIndex(e => e.ProgramId)
                .HasDatabaseName("IX_Initiatives_ProgramId");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Initiatives_Status");

            builder.HasIndex(e => e.Owner)
                .HasDatabaseName("IX_Initiatives_Owner");

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
