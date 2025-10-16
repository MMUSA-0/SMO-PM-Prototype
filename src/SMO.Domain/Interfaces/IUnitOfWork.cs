namespace SMO.Domain.Interfaces;

/// <summary>
/// Unit of Work interface for coordinating multiple repository operations within a single transaction.
/// Implements the Unit of Work pattern to ensure data consistency and transaction management.
/// </summary>
/// <remarks>
/// Design Principles:
/// - Single transaction boundary for multiple operations
/// - Repository factory pattern for accessing repositories
/// - Explicit save operations (no auto-save unless specified)
/// - Supports both synchronous and asynchronous persistence
///
/// Key Responsibilities:
/// - Manages database transaction lifecycle
/// - Coordinates changes across multiple entities/repositories
/// - Ensures all-or-nothing transaction semantics
/// - Provides repository instances on demand
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
///     public async Task&lt;int&gt; CreateObjectiveWithKPIsAsync(StrategicObjective objective, List&lt;KPI&gt; kpis)
///     {
///         // Get repositories
///         var objectiveRepo = _unitOfWork.Repository&lt;StrategicObjective&gt;();
///         var kpiRepo = _unitOfWork.Repository&lt;KPI&gt;();
///
///         // Perform operations (not yet persisted)
///         var createdObjective = await objectiveRepo.InsertAsync(objective);
///
///         foreach (var kpi in kpis)
///         {
///             kpi.StrategicObjectiveId = createdObjective.Id;
///             await kpiRepo.InsertAsync(kpi);
///         }
///
///         // Commit all changes in a single transaction
///         return await _unitOfWork.SaveChangesAsync();
///     }
/// }
/// </code>
///
/// Transaction Behavior:
/// - All operations are tracked but not persisted until SaveChanges/SaveChangesAsync is called
/// - If SaveChanges fails, all changes are rolled back automatically
/// - Multiple SaveChanges calls are supported (for batch operations)
/// - BaseDbContext automatically handles audit fields (CreatedBy, UpdatedBy, etc.)
///
/// Best Practices:
/// - Always call SaveChangesAsync after completing all operations
/// - Use try-catch to handle transaction failures
/// - Prefer Repository methods with autoSave=false and explicit SaveChangesAsync
/// - Don't call SaveChanges for individual operations (performance penalty)
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Synchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>The number of entities written to the database.</returns>
    /// <remarks>
    /// This method:
    /// - Persists all tracked changes (inserts, updates, deletes)
    /// - Automatically sets audit fields (CreatedBy, UpdatedBy, etc.) via BaseDbContext
    /// - Returns 0 if no changes were detected
    /// - Throws DbUpdateException if database constraints are violated
    ///
    /// WARNING: Prefer SaveChangesAsync for better performance and scalability.
    /// </remarks>
    int SaveChanges();

    /// <summary>
    /// Asynchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains the number of entities written to the database.</returns>
    /// <remarks>
    /// This method:
    /// - Persists all tracked changes (inserts, updates, deletes)
    /// - Automatically sets audit fields (CreatedBy, UpdatedBy, etc.) via BaseDbContext
    /// - Returns 0 if no changes were detected
    /// - Throws DbUpdateException if database constraints are violated
    /// - Does not block the calling thread (async/await pattern)
    ///
    /// This is the RECOMMENDED method for saving changes in web applications.
    ///
    /// Example:
    /// <code>
    /// try
    /// {
    ///     int affectedRows = await _unitOfWork.SaveChangesAsync();
    ///     _logger.LogInformation("Successfully saved {Count} changes", affectedRows);
    /// }
    /// catch (DbUpdateException ex)
    /// {
    ///     _logger.LogError(ex, "Failed to save changes");
    ///     throw;
    /// }
    /// </code>
    /// </remarks>
    Task<int> SaveChangesAsync();

    /// <summary>
    /// Gets a repository instance for the specified entity type.
    /// Acts as a factory method for creating/retrieving repositories on demand.
    /// </summary>
    /// <typeparam name="TEntity">The entity type for the repository. Must be a class.</typeparam>
    /// <returns>An IRepository instance for the specified entity type.</returns>
    /// <remarks>
    /// Repository Lifecycle:
    /// - Repositories are typically created on-demand per request
    /// - All repositories share the same DbContext instance within the UnitOfWork
    /// - Changes made through different repositories are coordinated by the same transaction
    ///
    /// Example:
    /// <code>
    /// // Get multiple repositories for a complex operation
    /// var initiativeRepo = _unitOfWork.Repository&lt;Initiative&gt;();
    /// var milestoneRepo = _unitOfWork.Repository&lt;InitiativeMilestone&gt;();
    /// var kpiRepo = _unitOfWork.Repository&lt;KPI&gt;();
    ///
    /// // Perform operations
    /// await initiativeRepo.InsertAsync(initiative);
    /// await milestoneRepo.InsertAsync(milestone);
    /// await kpiRepo.InsertAsync(kpi);
    ///
    /// // Save all changes in a single transaction
    /// await _unitOfWork.SaveChangesAsync();
    /// </code>
    /// </remarks>
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
}
