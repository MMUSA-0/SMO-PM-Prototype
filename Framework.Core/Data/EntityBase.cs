namespace Framework.Core.Data;

/// <summary>
/// Abstract base class for all entities in the system.
/// Provides the fundamental Id property required by all domain entities.
/// </summary>
/// <typeparam name="TKey">The type of the entity's primary key (e.g., int, Guid, long)</typeparam>
/// <remarks>
/// Usage Examples:
/// - For integer primary keys: public class User : EntityBase&lt;int&gt;
/// - For GUID primary keys: public class Order : EntityBase&lt;Guid&gt;
/// - For long primary keys: public class Log : EntityBase&lt;long&gt;
///
/// This class is abstract and cannot be instantiated directly.
/// All domain entities should inherit from either EntityBase, FullAuditedEntityBase, or LookupEntityBase.
/// </remarks>
public abstract class EntityBase<TKey> : IEntityBase<TKey>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// This is typically the primary key in the database.
    /// </summary>
    public TKey Id { get; set; }
}
