using Framework.Core.Data.Repositories;
using Microsoft.EntityFrameworkCore.Query;
using PagedList.Core;
using System.Linq.Expressions;

namespace SMO.Domain.Interfaces;

/// <summary>
/// Generic repository interface providing comprehensive data access operations.
/// Implements repository pattern with rich querying capabilities.
/// </summary>
/// <typeparam name="TEntity">The entity type this repository manages. Must be a class.</typeparam>
/// <remarks>
/// Design Principles:
/// - Generic repository pattern for CRUD operations
/// - Expression-based querying for flexibility
/// - No CQRS - single repository handles reads and writes
/// - Support for eager loading, filtering, and pagination
/// - Tracking control for performance optimization
///
/// Usage Example:
/// <code>
/// public class StrategicObjectiveAppService
/// {
///     private readonly IUnitOfWork _unitOfWork;
///
///     public StrategicObjectiveAppService(IUnitOfWork unitOfWork)
///     {
///         _unitOfWork = unitOfWork;
///     }
///
///     public async Task&lt;List&lt;StrategicObjective&gt;&gt; GetActiveObjectivesAsync()
///     {
///         var repo = _unitOfWork.Repository&lt;StrategicObjective&gt;();
///         return await repo.GetAsync(
///             predicate: x => x.IsActive,
///             orderBy: q => q.OrderBy(x => x.Order),
///             includes: new List&lt;Expression&lt;Func&lt;StrategicObjective, object&gt;&gt;&gt; { x => x.Pillar }
///         );
///     }
/// }
/// </code>
/// </remarks>
public interface IRepository<TEntity> : IRepositoryBase<IAppDbContext, TEntity>  where TEntity : class
{
    /// <summary>
    /// Gets a queryable collection of entities with change tracking enabled.
    /// Use this for queries that will modify entities.
    /// </summary>
    /// <remarks>
    /// WARNING: Use TableNoTracking for read-only queries to improve performance.
    /// </remarks>
    IQueryable<TEntity> Table { get; }

    /// <summary>
    /// Gets a queryable collection of entities with change tracking disabled.
    /// Use this for read-only queries to improve performance.
    /// </summary>
    /// <remarks>
    /// This is the preferred approach for most queries (reports, listings, etc.)
    /// Maps to EF Core's AsNoTracking().
    /// </remarks>
    IQueryable<TEntity> TableNoTracking { get; }

    #region CRUD Operations

    /// <summary>
    /// Inserts a new entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert. Cannot be null.</param>
    /// <param name="autoSave">If true, immediately persists changes to the database. Default is false.</param>
    /// <returns>The inserted entity with database-generated values (e.g., Id).</returns>
    /// <remarks>
    /// If autoSave is false, you must call IUnitOfWork.SaveChangesAsync() to persist changes.
    /// </remarks>
    TEntity Insert(TEntity entity, bool autoSave = false);

    /// <summary>
    /// Asynchronously inserts a new entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert. Cannot be null.</param>
    /// <param name="autoSave">If true, immediately persists changes to the database. Default is false.</param>
    /// <returns>The inserted entity with database-generated values (e.g., Id).</returns>
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update. Must have a valid Id.</param>
    /// <param name="autoSave">If true, immediately persists changes to the database. Default is false.</param>
    /// <returns>The updated entity.</returns>
    /// <remarks>
    /// The entity must be tracked by the context or attached before updating.
    /// If autoSave is false, you must call IUnitOfWork.SaveChangesAsync() to persist changes.
    /// </remarks>
    TEntity Update(TEntity entity, bool autoSave = false);

    /// <summary>
    /// Deletes an entity from the repository (hard delete).
    /// </summary>
    /// <param name="entity">The entity to delete. Must have a valid Id.</param>
    /// <param name="autoSave">If true, immediately persists changes to the database. Default is false.</param>
    /// <remarks>
    /// This performs a hard delete (removes from database).
    /// For soft deletion, update the IsActive property instead (for LookupEntityBase entities).
    /// If autoSave is false, you must call IUnitOfWork.SaveChangesAsync() to persist changes.
    /// </remarks>
    void Delete(TEntity entity, bool autoSave = false);

    #endregion

    #region Querying

    /// <summary>
    /// Gets an entity by its primary key.
    /// </summary>
    /// <param name="id">The primary key value. Can be int, Guid, long, etc.</param>
    /// <returns>The entity if found; otherwise null.</returns>
    Task<TEntity?> GetByIdAsync(object id);

    /// <summary>
    /// Gets a list of entities matching the specified criteria.
    /// </summary>
    /// <param name="predicate">Optional filter expression (WHERE clause). Null returns all entities.</param>
    /// <param name="orderBy">Optional ordering function (ORDER BY clause). Null returns unordered results.</param>
    /// <param name="includes">Optional list of navigation properties to eager load (JOIN clauses).</param>
    /// <param name="disableTracking">If true, disables change tracking for better performance. Default is true.</param>
    /// <returns>A list of entities matching the criteria.</returns>
    /// <remarks>
    /// Example:
    /// <code>
    /// var objectives = await repo.GetAsync(
    ///     predicate: x => x.PillarId == 1 && x.IsActive,
    ///     orderBy: q => q.OrderBy(x => x.Order).ThenBy(x => x.NameEn),
    ///     includes: new List&lt;Expression&lt;Func&lt;StrategicObjective, object&gt;&gt;&gt; { x => x.Pillar, x => x.Initiatives },
    ///     disableTracking: true
    /// );
    /// </code>
    /// </remarks>
    Task<List<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true);

    /// <summary>
    /// Gets a single entity with deep relationship loading support.
    /// Allows loading nested navigation properties (e.g., entity.Related.Nested).
    /// </summary>
    /// <param name="predicate">Optional filter expression. Null returns first entity.</param>
    /// <param name="include">Optional include expression for deep navigation property loading.</param>
    /// <param name="disableTracking">If true, disables change tracking for better performance. Default is true.</param>
    /// <returns>The first entity matching the criteria; null if not found.</returns>
    /// <remarks>
    /// Use this for complex object graphs requiring multiple levels of eager loading.
    /// Example:
    /// <code>
    /// var initiative = await repo.GetSingleWithDeepRelationsAsync(
    ///     predicate: x => x.Id == 1,
    ///     include: source => source
    ///         .Include(x => x.Program)
    ///             .ThenInclude(p => p.Pillar)
    ///         .Include(x => x.Objectives)
    ///             .ThenInclude(o => o.KPIs)
    /// );
    /// </code>
    /// </remarks>
    Task<TEntity?> GetSingleWithDeepRelationsAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true);

    #endregion

    #region Pagination

    /// <summary>
    /// Searches and returns a paginated result set with filtering support.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (1-based index).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="orderBy">Required ordering function. Must not be null.</param>
    /// <param name="filters">Optional collection of filter expressions (combined with AND logic).</param>
    /// <param name="includes">Optional navigation properties to eager load.</param>
    /// <returns>A PagedList containing the requested page of results and pagination metadata.</returns>
    /// <remarks>
    /// PagedList includes:
    /// - Items: The current page items
    /// - TotalItemCount: Total number of items across all pages
    /// - PageCount: Total number of pages
    /// - PageNumber: Current page number
    /// - PageSize: Items per page
    /// - HasPreviousPage, HasNextPage: Navigation flags
    ///
    /// Example:
    /// <code>
    /// var pagedResults = repo.SearchWithFilters(
    ///     pageNumber: 1,
    ///     pageSize: 20,
    ///     orderBy: q => q.OrderByDescending(x => x.CreatedOn),
    ///     filters: new[] {
    ///         (Expression&lt;Func&lt;Initiative, bool&gt;&gt;)(x => x.IsActive),
    ///         (Expression&lt;Func&lt;Initiative, bool&gt;&gt;)(x => x.StatusId == 1)
    ///     },
    ///     includes: x => x.Program, x => x.Owner
    /// );
    /// </code>
    /// </remarks>
    //PagedList<TEntity> SearchWithFilters(
    //    int pageNumber,
    //    int pageSize,
    //    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
    //    IEnumerable<Expression<Func<TEntity, bool>>>? filters = null,
    //    params Expression<Func<TEntity, object>>[] includes);

    #endregion
}
