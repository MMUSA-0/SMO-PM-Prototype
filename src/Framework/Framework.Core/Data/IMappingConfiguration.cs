using Microsoft.EntityFrameworkCore;

namespace Framework.Core.Data;

/// <summary>
/// Interface for applying entity type configurations to the model builder.
/// Implemented by EntityTypeConfiguration and EnumMapping base classes.
/// </summary>
/// <remarks>
/// Design Purpose:
/// - Enables auto-discovery pattern in AppDbContext.OnModelCreating
/// - Allows unified handling of entity configurations and enum mappings
/// - Supports runtime reflection to find and apply all configurations automatically
///
/// Implementations:
/// - EntityTypeConfiguration&lt;TEntity&gt;: Fluent API configuration for entities
/// - EnumMapping&lt;TEnum&gt;: Lookup table mapping for enumerations
///
/// Usage Example:
/// <code>
/// // In AppDbContext.OnModelCreating
/// var configurations = Assembly.GetExecutingAssembly().GetTypes()
///     .Where(type => typeof(IMappingConfiguration).IsAssignableFrom(type) && !type.IsAbstract);
///
/// foreach (var configurationType in configurations)
/// {
///     var configuration = (IMappingConfiguration)Activator.CreateInstance(configurationType);
///     configuration.ApplyConfiguration(modelBuilder);
/// }
/// </code>
/// </remarks>
public interface IMappingConfiguration
{
    /// <summary>
    /// Applies the entity type configuration to the specified model builder.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    void ApplyConfiguration(ModelBuilder modelBuilder);
}
