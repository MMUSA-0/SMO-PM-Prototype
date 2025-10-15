using Microsoft.EntityFrameworkCore;

namespace Framework.Core.Data;

/// <summary>
/// Abstract base class for mapping enumerations to database lookup tables.
/// Automatically seeds the database with enum values on migration.
/// </summary>
/// <typeparam name="TEnum">The enumeration type to map. Must be an enum.</typeparam>
/// <remarks>
/// Design Purpose:
/// - Map C# enums to database lookup tables
/// - Automatic database seeding with enum values
/// - Maintains referential integrity between transactional tables and enum lookups
/// - Auto-discovery: AppDbContext finds and applies all enum mappings
///
/// When to Use:
/// - Status enumerations (Active, Completed, Cancelled, etc.)
/// - Type/Category enumerations
/// - Fixed classification enumerations
/// - Any enum that needs to be referenced by foreign key
///
/// Benefits:
/// - Type safety in C# code (use enum instead of magic numbers/strings)
/// - Database referential integrity (foreign key constraints)
/// - Automatic synchronization between code and database
/// - Query optimization (integer comparisons vs. string comparisons)
///
/// Usage Example:
/// <code>
/// // 1. Define enum in Domain layer
/// public enum InitiativeStatus
/// {
///     Draft = 1,
///     UnderReview = 2,
///     Approved = 3,
///     InProgress = 4,
///     Completed = 5,
///     Cancelled = 6
/// }
///
/// // 2. Create lookup entity
/// public class InitiativeStatusLookup : LookupEntityBase&lt;int&gt;
/// {
///     // NameAr, NameEn, IsActive, Order inherited from LookupEntityBase
/// }
///
/// // 3. Create enum mapping in Infrastructure
/// public class InitiativeStatusMapping : EnumMapping&lt;InitiativeStatus&gt;
/// {
///     // No implementation needed - base class handles everything
///     // Lookup table will be created and seeded automatically
/// }
///
/// // 4. Use in entity
/// public class Initiative
/// {
///     public int StatusId { get; set; }  // Foreign key to InitiativeStatusLookup
///     public InitiativeStatus Status => (InitiativeStatus)StatusId;  // Computed property
/// }
/// </code>
///
/// Database Schema:
/// - Creates table named {EnumName}Lookup (e.g., InitiativeStatusLookup)
/// - Id column matches enum integer values
/// - NameAr and NameEn must be manually set (or use resource files)
/// - IsActive and Order columns for lookup management
///
/// Auto-Discovery:
/// - AppDbContext.OnModelCreating scans for EnumMapping types
/// - All non-generic, non-abstract derived classes are automatically applied
/// - Enum values are seeded during migrations
///
/// Note: This is a placeholder base class. Concrete implementation will be added in future stories.
/// </remarks>
public abstract class EnumMapping<TEnum> : IMappingConfiguration where TEnum : struct, Enum
{
    /// <summary>
    /// Applies the enum mapping configuration to the model builder.
    /// Called automatically by AppDbContext during model creation.
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure.</param>
    /// <remarks>
    /// Implementation will:
    /// - Create a lookup table for the enum
    /// - Seed the table with enum values
    /// - Configure relationships if needed
    ///
    /// NOTE: Full implementation will be added when entities are created in future stories.
    /// </remarks>
    public virtual void ApplyConfiguration(ModelBuilder modelBuilder)
    {
        // Placeholder implementation
        // Full enum mapping logic will be implemented when entities are created
        // This ensures the auto-discovery pattern works without causing errors

        // Future implementation will:
        // 1. Get enum type name (e.g., "InitiativeStatus")
        // 2. Create/configure entity type {EnumName}Lookup
        // 3. Seed data with enum values
        // 4. Configure primary key and properties
    }
}
