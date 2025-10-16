using Framework.Core.Data;
using SMO.Domain.Interfaces;

namespace SMO.Infrastructure.Data;

/// <summary>
/// Unit of Work implementation for the SMO application.
/// Coordinates transactions across multiple repositories within a single database context.
/// </summary>
/// <remarks>
/// Architecture:
/// - Inherits from UnitOfWorkBase (Framework.Core) for common UoW logic
/// - Implements IUnitOfWork (SMO.Domain) for domain-specific contract
/// - Uses IAppDbContext for database access
/// - Sealed class to prevent further inheritance
///
/// Design Principles:
/// - Single transaction boundary for multiple operations
/// - Explicit save operations (no auto-save unless specified)
/// - Repository factory pattern for on-demand repository creation
/// - All repositories share the same DbContext instance
///
/// Transaction Behavior:
/// - All repository operations are tracked but not persisted until SaveChangesAsync
/// - If SaveChangesAsync fails, all changes are rolled back automatically
/// - Multiple SaveChangesAsync calls are supported (for batch operations)
/// - BaseDbContext automatically handles audit fields (CreatedBy, UpdatedBy, etc.)
///
/// Usage Example:
/// <code>
/// public class InitiativeAppService
/// {
///     private readonly IUnitOfWork _unitOfWork;
///     private readonly ILogger&lt;InitiativeAppService&gt; _logger;
///
///     public InitiativeAppService(IUnitOfWork unitOfWork, ILogger&lt;InitiativeAppService&gt; logger)
///     {
///         _unitOfWork = unitOfWork;
///         _logger = logger;
///     }
///
///     public async Task&lt;InitiativeDto&gt; CreateInitiativeWithMilestonesAsync(CreateInitiativeDto dto)
///     {
///         try
///         {
///             // Get repositories
///             var initiativeRepo = _unitOfWork.Repository&lt;Initiative&gt;();
///             var milestoneRepo = _unitOfWork.Repository&lt;InitiativeMilestone&gt;();
///             var kpiRepo = _unitOfWork.Repository&lt;KPI&gt;();
///
///             // Create initiative
///             var initiative = _mapper.Map&lt;Initiative&gt;(dto);
///             await initiativeRepo.InsertAsync(initiative);
///
///             // Create milestones
///             foreach (var milestoneDto in dto.Milestones)
///             {
///                 var milestone = _mapper.Map&lt;InitiativeMilestone&gt;(milestoneDto);
///                 milestone.InitiativeId = initiative.Id;
///                 await milestoneRepo.InsertAsync(milestone);
///             }
///
///             // Create KPIs
///             foreach (var kpiDto in dto.KPIs)
///             {
///                 var kpi = _mapper.Map&lt;KPI&gt;(kpiDto);
///                 kpi.InitiativeId = initiative.Id;
///                 await kpiRepo.InsertAsync(kpi);
///             }
///
///             // Commit all changes in a single transaction
///             int affectedRows = await _unitOfWork.SaveChangesAsync();
///             _logger.LogInformation("Successfully created initiative with {Count} related entities", affectedRows);
///
///             return _mapper.Map&lt;InitiativeDto&gt;(initiative);
///         }
///         catch (DbUpdateException ex)
///         {
///             _logger.LogError(ex, "Failed to create initiative with milestones");
///             throw new ApplicationException("Failed to save initiative. Please check the data and try again.", ex);
///         }
///     }
///
///     public async Task&lt;bool&gt; DeleteInitiativeAsync(int id)
///     {
///         try
///         {
///             var initiativeRepo = _unitOfWork.Repository&lt;Initiative&gt;();
///             var initiative = await initiativeRepo.GetByIdAsync(id);
///
///             if (initiative == null)
///             {
///                 return false;
///             }
///
///             // Delete cascade will handle related milestones and KPIs (configured in EF Core)
///             initiativeRepo.Delete(initiative);
///             await _unitOfWork.SaveChangesAsync();
///
///             _logger.LogInformation("Successfully deleted initiative {Id}", id);
///             return true;
///         }
///         catch (DbUpdateException ex)
///         {
///             _logger.LogError(ex, "Failed to delete initiative {Id}", id);
///             throw;
///         }
///     }
/// }
/// </code>
///
/// Best Practices:
/// - Always wrap SaveChangesAsync in try-catch for error handling
/// - Use repository methods with autoSave=false and explicit SaveChangesAsync
/// - Don't call SaveChangesAsync for individual operations (performance penalty)
/// - Log affected row counts for auditing
/// - Handle DbUpdateException for database constraint violations
///
/// Dependency Injection Configuration:
/// <code>
/// // In ServiceCollectionExtensions.cs
/// services.AddScoped&lt;IUnitOfWork, UnitOfWork&gt;();
/// </code>
/// </remarks>
public sealed class UnitOfWork : UnitOfWorkBase<IAppDbContext>, IUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of UnitOfWork with the specified context.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when context is null.</exception>
    public UnitOfWork(IAppDbContext context) : base(context)
    {
        // Context is stored in base class and shared by all repositories
    }

    /// <summary>
    /// Gets a repository instance for the specified entity type.
    /// Creates a new Repository instance that shares the same DbContext.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository. Must be a class.</typeparam>
    /// <returns>An IRepository instance for the specified entity type.</returns>
    /// <remarks>
    /// Repository Lifecycle:
    /// - Repositories are created on-demand per call
    /// - All repositories share the same IAppDbContext instance
    /// - Changes made through different repositories are coordinated by the same transaction
    /// - Repositories are lightweight and don't need to be cached
    ///
    /// Example:
    /// <code>
    /// // Get multiple repositories for a complex operation
    /// var repo1 = _unitOfWork.Repository&lt;StrategicObjective&gt;();
    /// var repo2 = _unitOfWork.Repository&lt;Initiative&gt;();
    /// var repo3 = _unitOfWork.Repository&lt;KPI&gt;();
    ///
    /// // All three repositories share the same DbContext
    /// // Changes are coordinated by the same transaction
    /// await _unitOfWork.SaveChangesAsync();
    /// </code>
    /// </remarks>
    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        // Create and return a new Repository instance
        // The Repository constructor receives the shared IAppDbContext
        return new Repository<TEntity>(Context);
    }

    // SaveChanges and SaveChangesAsync methods are inherited from UnitOfWorkBase
    // They delegate to Context.SaveChanges/SaveChangesAsync which:
    // - Detects all tracked changes across all repositories
    // - Generates SQL commands (INSERT, UPDATE, DELETE)
    // - Executes within a transaction
    // - Updates audit fields via BaseDbContext
    // - Returns the number of affected rows
}
