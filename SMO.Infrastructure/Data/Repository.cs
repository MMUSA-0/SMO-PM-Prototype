using Framework.Core.Data;
using SMO.Domain.Interfaces;

namespace SMO.Infrastructure.Data;

/// <summary>
/// Generic repository implementation for the SMO application.
/// Provides comprehensive data access operations for all entity types.
/// </summary>
/// <typeparam name="TEntity">The entity type this repository manages. Must be a class.</typeparam>
/// <remarks>
/// Architecture:
/// - Inherits from RepositoryBase (Framework.Core) for common repository logic
/// - Implements IRepository (SMO.Domain) for domain-specific contract
/// - Uses IAppDbContext for database access
/// - Supports all entity types in the domain model
///
/// Design Principles:
/// - Generic repository pattern (no entity-specific repositories unless needed)
/// - Expression-based querying for flexibility
/// - Tracking control for performance optimization
/// - Consistent data access patterns across the application
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
///     public async Task&lt;StrategicObjective&gt; CreateObjectiveAsync(StrategicObjective objective)
///     {
///         var repo = _unitOfWork.Repository&lt;StrategicObjective&gt;();
///
///         // Insert without auto-save
///         await repo.InsertAsync(objective, autoSave: false);
///
///         // Commit transaction
///         await _unitOfWork.SaveChangesAsync();
///
///         return objective;
///     }
///
///     public async Task&lt;List&lt;StrategicObjective&gt;&gt; GetActiveBajectivesAsync(int pillarId)
///     {
///         var repo = _unitOfWork.Repository&lt;StrategicObjective&gt;();
///
///         return await repo.GetAsync(
///             predicate: x => x.PillarId == pillarId && x.IsActive,
///             orderBy: q => q.OrderBy(x => x.Order),
///             includes: new List&lt;Expression&lt;Func&lt;StrategicObjective, object&gt;&gt;&gt;
///             {
///                 x => x.Pillar,
///                 x => x.Initiatives
///             },
///             disableTracking: true
///         );
///     }
/// }
/// </code>
///
/// When to Create Specialized Repositories:
/// - Complex queries requiring stored procedures (use IRepositoryBaseStoreProcedure)
/// - Entity-specific business logic that doesn't belong in services
/// - Performance-critical operations requiring custom optimization
///
/// For most entities, this generic repository is sufficient.
/// Do NOT create specialized repositories unless there's a clear need.
///
/// Dependency Injection Configuration:
/// <code>
/// // In ServiceCollectionExtensions.cs
/// services.AddScoped(typeof(IRepository&lt;&gt;), typeof(Repository&lt;&gt;));
/// </code>
/// </remarks>
public class Repository<TEntity> : RepositoryBase<IAppDbContext, TEntity>, IRepository<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of Repository with the specified context.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public Repository(IAppDbContext context) : base(context)
    {
        // All logic inherited from RepositoryBase
        // No additional implementation needed for generic repository
        // Specialized logic can be added here if needed in the future
    }

    // All CRUD and querying methods are inherited from RepositoryBase<IAppDbContext, TEntity>
    // and satisfy the IRepository<TEntity> contract:
    // - Table, TableNoTracking properties
    // - Insert, InsertAsync, Update, Delete methods
    // - GetByIdAsync, GetAsync, GetSingleWithDeepRelationsAsync methods
    // - SearchWithFilters method for pagination

    // NOTE: Override base class methods here if entity-specific customization is needed
    // Example:
    // public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = false)
    // {
    //     // Custom logic before insert
    //     var result = await base.InsertAsync(entity, autoSave);
    //     // Custom logic after insert
    //     return result;
    // }
}
