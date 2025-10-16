namespace Framework.Core.Data;

/// <summary>
/// Abstract base class for entities that require full audit tracking.
/// Extends EntityBase with creation and modification audit fields.
/// </summary>
/// <typeparam name="TKey">The type of the entity's primary key (e.g., int, Guid, long)</typeparam>
/// <remarks>
/// Usage Example:
/// <code>
/// public class StrategicObjective : FullAuditedEntityBase&lt;int&gt;
/// {
///     public string NameAr { get; set; }
///     public string NameEn { get; set; }
///     // Additional properties...
/// }
/// </code>
///
/// Audit Fields Behavior:
/// - CreatedBy and CreatedOn: Automatically populated by BaseDbContext on entity creation
/// - UpdatedBy and UpdatedOn: Automatically populated by BaseDbContext on entity modification
/// - These fields are managed by SaveChanges/SaveChangesAsync in the DbContext
///
/// This class should be used for:
/// - Domain entities that require audit trails
/// - Entities where tracking creation and modification is important
/// - Most business entities (Initiatives, KPIs, Programs, etc.)
///
/// DO NOT use for:
/// - Simple lookup tables (use LookupEntityBase instead)
/// - Entities without audit requirements (use EntityBase instead)
/// </remarks>
public abstract class FullAuditedEntityBase<TKey> : EntityBase<TKey>
{
    /// <summary>
    /// Gets or sets the username or user ID who created this entity.
    /// Automatically populated by BaseDbContext during SaveChanges.
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the UTC date and time when this entity was created.
    /// Automatically populated by BaseDbContext during SaveChanges.
    /// </summary>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets the username or user ID who last updated this entity.
    /// Automatically populated by BaseDbContext during SaveChanges.
    /// Null if the entity has never been modified after creation.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Gets or sets the UTC date and time when this entity was last updated.
    /// Automatically populated by BaseDbContext during SaveChanges.
    /// Null if the entity has never been modified after creation.
    /// </summary>
    public DateTime? UpdatedOn { get; set; }
}
