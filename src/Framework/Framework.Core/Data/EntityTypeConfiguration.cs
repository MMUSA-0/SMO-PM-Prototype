using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Framework.Core.Data;

/// <summary>
/// Abstract base class for entity type configurations using Fluent API.
/// Provides a strongly-typed way to configure entity mappings separate from the DbContext.
/// </summary>
/// <typeparam name="TEntity">The entity type to configure. Must be a class.</typeparam>
/// <remarks>
/// Design Purpose:
/// - Separation of concerns: Entity configurations separate from DbContext
/// - Auto-discovery: AppDbContext automatically finds and applies all configurations
/// - Organization: Each entity has its own configuration class
/// - Fluent API: Strongly-typed configuration instead of Data Annotations
///
/// Configuration Capabilities:
/// - Table name and schema mapping
/// - Primary key configuration
/// - Column names, types, and constraints
/// - Relationships (one-to-many, many-to-many, one-to-one)
/// - Indexes and unique constraints
/// - Default values and computed columns
/// - Query filters and value conversions
///
/// Usage Example:
/// <code>
/// // Create in SMO.Infrastructure/Mapping/Vision/PillarMapping.cs
/// public class PillarMapping : EntityTypeConfiguration&lt;Pillar&gt;
/// {
///     public override void Configure(EntityTypeBuilder&lt;Pillar&gt; builder)
///     {
///         // Table mapping
///         builder.ToTable("Pillars");
///
///         // Primary key
///         builder.HasKey(x => x.Id);
///
///         // Properties
///         builder.Property(x => x.NameAr)
///             .IsRequired()
///             .HasMaxLength(200);
///
///         builder.Property(x => x.NameEn)
///             .IsRequired()
///             .HasMaxLength(200);
///
///         builder.Property(x => x.Order)
///             .IsRequired();
///
///         // Relationships
///         builder.HasMany(x => x.Themes)
///             .WithOne(x => x.Pillar)
///             .HasForeignKey(x => x.PillarId)
///             .OnDelete(DeleteBehavior.Restrict);
///
///         // Indexes
///         builder.HasIndex(x => x.Order)
///             .IsUnique();
///     }
/// }
/// </code>
///
/// Auto-Discovery:
/// - AppDbContext.OnModelCreating scans the assembly for EntityTypeConfiguration types
/// - All non-generic, non-abstract derived classes are automatically instantiated and applied
/// - No need to manually register each configuration in OnModelCreating
///
/// Advantages over Data Annotations:
/// - Complex relationships easier to express
/// - No pollution of domain entities with mapping attributes
/// - Centralized configuration per entity
/// - Better support for advanced EF Core features
/// </remarks>
public abstract class EntityTypeConfiguration<TEntity> : IMappingConfiguration, IEntityTypeConfiguration<TEntity> where TEntity : class
{
    /// <summary>
    /// Configures the entity type using Fluent API.
    /// Override this method to define table mappings, relationships, indexes, etc.
    /// </summary>
    /// <param name="builder">The entity type builder for TEntity.</param>
    public abstract void Configure(EntityTypeBuilder<TEntity> builder);

    /// <summary>
    /// Applies the entity type configuration to the model builder.
    /// Called automatically by AppDbContext during model creation.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    public void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(this);
    }

    /// <summary>
    /// Configures the entity type. Required by IEntityTypeConfiguration interface.
    /// Routes to the abstract Configure method.
    /// </summary>
    /// <param name="builder">The entity type builder for TEntity.</param>
    void IEntityTypeConfiguration<TEntity>.Configure(EntityTypeBuilder<TEntity> builder)
    {
        Configure(builder);
    }
}
