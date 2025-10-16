using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PagedList.Core;
using System.Linq.Expressions;

namespace Framework.Core.Data;

/// <summary>
/// Abstract base class providing generic repository implementation.
/// Contains common data access logic reusable across all repositories.
/// </summary>
/// <typeparam name="TDbContext">The database context type implementing IBaseDbContext.</typeparam>
/// <typeparam name="TEntity">The entity type this repository manages. Must be a class.</typeparam>
/// <remarks>
/// Design Purpose:
/// - Centralized repository logic in framework layer
/// - Reduces code duplication across application-specific repositories
/// - Provides consistent data access patterns
/// - Domain-specific repositories inherit from this base class
///
/// Usage Example:
/// <code>
/// // In Infrastructure layer
/// public class Repository&lt;TEntity&gt; : RepositoryBase&lt;IAppDbContext, TEntity&gt;, IRepository&lt;TEntity&gt;
///     where TEntity : class
/// {
///     public Repository(IAppDbContext context) : base(context)
///     {
///     }
/// }
/// </code>
/// </remarks>
public abstract class RepositoryBase<TDbContext, TEntity>
    where TDbContext : IBaseDbContext
    where TEntity : class
{
    /// <summary>
    /// The database context instance.
    /// </summary>
    protected readonly TDbContext Context;

    /// <summary>
    /// The DbSet for the entity type.
    /// </summary>
    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// Initializes a new instance of RepositoryBase with the specified context.
    /// </summary>
    /// <param name="context">The database context.</param>
    protected RepositoryBase(TDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
    }

    /// <summary>
    /// Gets a queryable collection of entities with change tracking enabled.
    /// </summary>
    public virtual IQueryable<TEntity> Table => DbSet;

    /// <summary>
    /// Gets a queryable collection of entities with change tracking disabled.
    /// </summary>
    public virtual IQueryable<TEntity> TableNoTracking => DbSet.AsNoTracking();

    #region CRUD Operations

    /// <summary>
    /// Inserts a new entity into the repository.
    /// </summary>
    public virtual TEntity Insert(TEntity entity, bool autoSave = false)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        DbSet.Add(entity);

        if (autoSave)
        {
            Context.SaveChanges();
        }

        return entity;
    }

    /// <summary>
    /// Asynchronously inserts a new entity into the repository.
    /// </summary>
    public virtual async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        await DbSet.AddAsync(entity);

        if (autoSave)
        {
            await Context.SaveChangesAsync();
        }

        return entity;
    }

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    public virtual TEntity Update(TEntity entity, bool autoSave = false)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        Context.Entry(entity).State = EntityState.Modified;

        if (autoSave)
        {
            Context.SaveChanges();
        }

        return entity;
    }

    /// <summary>
    /// Deletes an entity from the repository (hard delete).
    /// </summary>
    public virtual void Delete(TEntity entity, bool autoSave = false)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        DbSet.Remove(entity);

        if (autoSave)
        {
            Context.SaveChanges();
        }
    }

    #endregion

    #region Querying

    /// <summary>
    /// Gets an entity by its primary key.
    /// </summary>
    public virtual async Task<TEntity?> GetByIdAsync(object id)
    {
        if (id == null) throw new ArgumentNullException(nameof(id));
        return await DbSet.FindAsync(id);
    }

    /// <summary>
    /// Gets a list of entities matching the specified criteria.
    /// </summary>
    public virtual async Task<List<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        List<Expression<Func<TEntity, object>>>? includes = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> query = disableTracking ? TableNoTracking : Table;

        // Apply predicate (WHERE clause)
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        // Apply includes (JOIN clauses)
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        // Apply ordering (ORDER BY clause)
        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

    /// <summary>
    /// Gets a single entity with deep relationship loading support.
    /// </summary>
    public virtual async Task<TEntity?> GetSingleWithDeepRelationsAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        bool disableTracking = true)
    {
        IQueryable<TEntity> query = disableTracking ? TableNoTracking : Table;

        // Apply deep includes (supports ThenInclude)
        if (include != null)
        {
            query = include(query);
        }

        // Apply predicate (WHERE clause)
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.FirstOrDefaultAsync();
    }

    #endregion

    #region Pagination

    /// <summary>
    /// Searches and returns a paginated result set with filtering support.
    /// </summary>
    public virtual PagedList<TEntity> SearchWithFilters(
        int pageNumber,
        int pageSize,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        IEnumerable<Expression<Func<TEntity, bool>>>? filters = null,
        params Expression<Func<TEntity, object>>[] includes)
    {
        if (orderBy == null) throw new ArgumentNullException(nameof(orderBy));
        if (pageNumber < 1) throw new ArgumentException("Page number must be >= 1", nameof(pageNumber));
        if (pageSize < 1) throw new ArgumentException("Page size must be >= 1", nameof(pageSize));

        IQueryable<TEntity> query = TableNoTracking;

        // Apply filters (combined with AND logic)
        if (filters != null)
        {
            query = filters.Aggregate(query, (current, filter) => current.Where(filter));
        }

        // Apply includes (JOIN clauses)
        if (includes != null && includes.Length > 0)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        // Apply ordering (required for pagination)
        query = orderBy(query);

        // Create paged result
        return new PagedList<TEntity>(query, pageNumber, pageSize);
    }

    #endregion
}
