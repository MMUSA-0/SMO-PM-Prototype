using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SMO.Domain.Entities.Shared;

namespace SMO.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Entity Framework configuration for shared Risk entity
    /// Maps to the Risks table in the database - shared across both modules
    /// </summary>
    public class RiskConfiguration : IEntityTypeConfiguration<Risk>
    {
        public void Configure(EntityTypeBuilder<Risk> builder)
        {
            // Table mapping
            builder.ToTable("Risks");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.Code)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(e => e.Title)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(e => e.TitleAr)
                .HasMaxLength(500);

            builder.Property(e => e.Description)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.DescriptionAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.Category)
                .HasMaxLength(100);

            builder.Property(e => e.Probability)
                .HasMaxLength(20);

            builder.Property(e => e.Impact)
                .HasMaxLength(20);

            builder.Property(e => e.RiskScore)
                .HasColumnType("decimal(5,2)");

            builder.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Open");

            builder.Property(e => e.MitigationPlan)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.MitigationPlanAr)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.ContingencyPlan)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.Owner)
                .HasMaxLength(256);

            builder.Property(e => e.EscalatedTo)
                .HasMaxLength(256);

            builder.Property(e => e.IdentifiedDate)
                .HasColumnType("date");

            builder.Property(e => e.ClosedDate)
                .HasColumnType("date");

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
            builder.HasMany(e => e.ProgramRisks)
                .WithOne()
                .HasForeignKey("RiskId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.InitiativeRisks)
                .WithOne()
                .HasForeignKey("RiskId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.PerformanceRisks)
                .WithOne(pr => pr.Risk)
                .HasForeignKey(pr => pr.RiskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Code)
                .IsUnique()
                .HasDatabaseName("IX_Risks_Code");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Risks_Status");

            builder.HasIndex(e => e.OriginModule)
                .HasDatabaseName("IX_Risks_OriginModule");

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
