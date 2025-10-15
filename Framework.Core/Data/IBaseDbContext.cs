using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Framework.Core.Data;

/// <summary>
/// Base database context interface defining core database operations.
/// All application-specific DbContext interfaces should inherit from this interface.
/// </summary>
/// <remarks>
/// Design Purpose:
/// - Abstraction layer between domain/application layers and EF Core implementation
/// - Enables dependency inversion (domain depends on abstraction, not concrete implementation)
/// - Facilitates unit testing through mocking
/// - Provides access to essential DbContext operations without exposing full EF Core API
///
/// Usage:
/// - Inherit this interface in domain-specific context interfaces (e.g., IAppDbContext)
/// - Implement this interface in concrete DbContext classes via BaseDbContext
///
/// Example:
/// <code>
/// // In Domain Layer
/// public interface IAppDbContext : IBaseDbContext
/// {
///     DbSet&lt;StrategicObjective&gt; StrategicObjectives { get; set; }
///     DbSet&lt;Initiative&gt; Initiatives { get; set; }
/// }
/// </code>
/// </remarks>
public interface IBaseDbContext
{
    /// <summary>
    /// Provides access to the ChangeTracker for inspecting and managing entity state.
    /// </summary>
    /// <remarks>
    /// The ChangeTracker tracks all changes to entities retrieved from or attached to the context.
    /// Useful for:
    /// - Inspecting entity states (Added, Modified, Deleted, Unchanged)
    /// - Manually managing entity state
    /// - Clearing tracked entities
    /// - Debugging tracking issues
    /// </remarks>
    ChangeTracker ChangeTracker { get; }

    /// <summary>
    /// Provides access to database-related operations.
    /// </summary>
    /// <remarks>
    /// The Database property exposes operations like:
    /// - Migrations (EnsureCreated, Migrate)
    /// - Transactions (BeginTransaction, CommitTransaction)
    /// - Raw SQL queries (ExecuteSqlRaw, SqlQuery)
    /// - Connection management
    /// </remarks>
    DatabaseFacade Database { get; }

    /// <summary>
    /// Synchronously saves all changes made in this context to the database.
    /// </summary>
    /// <returns>The number of entities written to the database.</returns>
    /// <remarks>
    /// This method:
    /// - Automatically detects all changes to tracked entities
    /// - Generates appropriate SQL commands (INSERT, UPDATE, DELETE)
    /// - Executes commands within a transaction
    /// - Updates audit fields (CreatedBy, UpdatedBy, etc.) in BaseDbContext implementation
    /// - Returns 0 if no changes were detected
    ///
    /// WARNING: Prefer SaveChangesAsync for better performance.
    /// </remarks>
    int SaveChanges();

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the number of entities written to the database.</returns>
    /// <remarks>
    /// This method:
    /// - Automatically detects all changes to tracked entities
    /// - Generates appropriate SQL commands (INSERT, UPDATE, DELETE)
    /// - Executes commands within a transaction
    /// - Updates audit fields (CreatedBy, UpdatedBy, etc.) in BaseDbContext implementation
    /// - Returns 0 if no changes were detected
    /// - Does not block the calling thread (async/await pattern)
    ///
    /// This is the RECOMMENDED method for saving changes in ASP.NET Core applications.
    /// </remarks>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a DbSet for the specified entity type.
    /// Provides access to querying and saving instances of TEntity.
    /// </summary>
    /// <typeparam name="TEntity">The entity type. Must be a class.</typeparam>
    /// <returns>A DbSet for the specified entity type.</returns>
    /// <remarks>
    /// DbSet represents a collection of entities that can be queried from the database.
    /// Operations on DbSet are translated to SQL queries by EF Core.
    ///
    /// Example:
    /// <code>
    /// // Get all active strategic objectives
    /// var objectives = await context.Set&lt;StrategicObjective&gt;()
    ///     .Where(x => x.IsActive)
    ///     .ToListAsync();
    /// </code>
    /// </remarks>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    /// <summary>
    /// Gets an EntityEntry for the given entity, providing access to change tracking information.
    /// </summary>
    /// <typeparam name="TEntity">The entity type. Must be a class.</typeparam>
    /// <param name="entity">The entity to get the entry for.</param>
    /// <returns>An EntityEntry providing access to change tracking and metadata.</returns>
    /// <remarks>
    /// EntityEntry provides:
    /// - Current entity state (Added, Modified, Deleted, Unchanged, Detached)
    /// - Original and current property values
    /// - Ability to manually set entity state
    /// - Navigation property metadata
    ///
    /// Example:
    /// <code>
    /// var entry = context.Entry(objective);
    /// entry.State = EntityState.Modified;
    /// entry.Property(x => x.NameEn).IsModified = true;
    /// </code>
    /// </remarks>
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
}
