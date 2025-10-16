namespace Framework.Core.Data;

/// <summary>
/// Abstract base class for Unit of Work pattern implementation.
/// Provides transaction coordination logic reusable across applications.
/// </summary>
/// <typeparam name="TDbContext">The database context type implementing IBaseDbContext.</typeparam>
/// <remarks>
/// Design Purpose:
/// - Centralized Unit of Work logic in framework layer
/// - Transaction coordination across multiple repositories
/// - Explicit save operations for better control
/// - Application-specific UnitOfWork classes inherit from this base
///
/// Key Responsibilities:
/// - Delegates SaveChanges to the DbContext
/// - Provides repository factory method (must be implemented by derived class)
/// - Ensures all repositories share the same DbContext instance
///
/// Usage Example:
/// <code>
/// // In Infrastructure layer
/// public sealed class UnitOfWork : UnitOfWorkBase&lt;IAppDbContext&gt;, IUnitOfWork
/// {
///     public UnitOfWork(IAppDbContext context) : base(context)
///     {
///     }
///
///     public override IRepository&lt;TEntity&gt; Repository&lt;TEntity&gt;()
///     {
///         return new Repository&lt;TEntity&gt;(Context);
///     }
/// }
/// </code>
/// </remarks>
public abstract class UnitOfWorkBase<TDbContext>
    where TDbContext : IBaseDbContext
{
    /// <summary>
    /// The database context instance.
    /// All repositories created by this UnitOfWork share this context.
    /// </summary>
    protected readonly TDbContext Context;

    /// <summary>
    /// Initializes a new instance of UnitOfWorkBase with the specified context.
    /// </summary>
    /// <param name="context">The database context.</param>
    protected UnitOfWorkBase(TDbContext context)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Synchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>The number of entities written to the database.</returns>
    /// <remarks>
    /// Delegates to DbContext.SaveChanges which:
    /// - Detects all changes to tracked entities
    /// - Generates appropriate SQL commands (INSERT, UPDATE, DELETE)
    /// - Executes commands within a transaction
    /// - Updates audit fields (CreatedBy, UpdatedBy, etc.) via BaseDbContext
    /// - Returns 0 if no changes were detected
    ///
    /// WARNING: Prefer SaveChangesAsync for better performance.
    /// </remarks>
    public virtual int SaveChanges()
    {
        return Context.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains the number of entities written to the database.</returns>
    /// <remarks>
    /// Delegates to DbContext.SaveChangesAsync which:
    /// - Detects all changes to tracked entities
    /// - Generates appropriate SQL commands (INSERT, UPDATE, DELETE)
    /// - Executes commands within a transaction
    /// - Updates audit fields (CreatedBy, UpdatedBy, etc.) via BaseDbContext
    /// - Returns 0 if no changes were detected
    /// - Does not block the calling thread (async/await pattern)
    ///
    /// This is the RECOMMENDED method for saving changes in web applications.
    ///
    /// Example:
    /// <code>
    /// var objectiveRepo = _unitOfWork.Repository&lt;StrategicObjective&gt;();
    /// var kpiRepo = _unitOfWork.Repository&lt;KPI&gt;();
    ///
    /// await objectiveRepo.InsertAsync(objective);
    /// await kpiRepo.InsertAsync(kpi);
    ///
    /// int affectedRows = await _unitOfWork.SaveChangesAsync();
    /// </code>
    /// </remarks>
    public virtual Task<int> SaveChangesAsync()
    {
        return Context.SaveChangesAsync();
    }

    // Note: Repository<TEntity>() factory method is intentionally NOT implemented here
    // because it needs to know the concrete repository type (e.g., Repository<TEntity>).
    // Derived classes must implement this method to return the correct repository instance.
    // Example implementation in derived class:
    // public override IRepository<TEntity> Repository<TEntity>()
    // {
    //     return new Repository<TEntity>(Context);
    // }
}
