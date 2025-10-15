namespace Framework.Core.Data;

/// <summary>
/// Base interface for all entities in the system.
/// Provides a generic Id property with flexible key type support.
/// </summary>
/// <typeparam name="TKey">The type of the entity's primary key (e.g., int, Guid, long)</typeparam>
public interface IEntityBase<TKey>
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    TKey Id { get; set; }
}
