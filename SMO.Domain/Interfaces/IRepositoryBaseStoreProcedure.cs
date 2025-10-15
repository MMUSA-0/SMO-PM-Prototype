namespace SMO.Domain.Interfaces;

/// <summary>
/// Repository interface for executing stored procedures and raw SQL commands.
/// Provides direct database access for complex queries and operations beyond EF Core LINQ.
/// </summary>
/// <remarks>
/// Design Purpose:
/// - Support for complex stored procedures (reporting, bulk operations, etc.)
/// - Execution of raw SQL for performance-critical queries
/// - Integration with legacy database objects
/// - Complex aggregations and calculations not easily expressed in LINQ
///
/// When to Use:
/// - Complex reporting queries with multiple joins and aggregations
/// - Bulk operations (updates, deletes) affecting thousands of records
/// - Database-specific features (window functions, CTEs, pivots)
/// - Legacy stored procedures that cannot be rewritten in LINQ
/// - Performance-critical queries requiring specific SQL optimization
///
/// When NOT to Use:
/// - Simple CRUD operations (use IRepository instead)
/// - Queries that can be expressed clearly in LINQ
/// - Operations requiring change tracking (use IRepository)
///
/// Usage Example:
/// <code>
/// public class DashboardAppService
/// {
///     private readonly IRepositoryBaseStoreProcedure _spRepository;
///
///     public DashboardAppService(IRepositoryBaseStoreProcedure spRepository)
///     {
///         _spRepository = spRepository;
///     }
///
///     public async Task&lt;List&lt;KPIPerformanceReport&gt;&gt; GetKPIPerformanceReportAsync(int pillarId, DateTime startDate, DateTime endDate)
///     {
///         return await _spRepository.ExecuteStoredProcedureAsync&lt;KPIPerformanceReport&gt;(
///             "[dbo].[SP_GetKPIPerformanceReport]",
///             new SqlParameter("@PillarId", pillarId),
///             new SqlParameter("@StartDate", startDate),
///             new SqlParameter("@EndDate", endDate)
///         );
///     }
/// }
/// </code>
///
/// Security Considerations:
/// - All methods accept SqlParameter objects to prevent SQL injection
/// - Never concatenate user input into SQL strings
/// - Always use parameterized queries
///
/// Future Implementation:
/// - Concrete implementation will be created in SMO.Infrastructure
/// - Will use EF Core's FromSqlRaw and ExecuteSqlRaw methods
/// - Will support both scalar results and entity collections
/// </remarks>
public interface IRepositoryBaseStoreProcedure
{
    /// <summary>
    /// Executes a stored procedure that returns a collection of entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map results to. Must be a class.</typeparam>
    /// <param name="storedProcedureName">The name of the stored procedure (e.g., "[dbo].[SP_GetActiveInitiatives]").</param>
    /// <param name="parameters">SQL parameters for the stored procedure. Use SqlParameter to prevent SQL injection.</param>
    /// <returns>A list of entities mapped from the stored procedure result set.</returns>
    /// <remarks>
    /// Requirements:
    /// - TEntity properties must match stored procedure column names (case-insensitive)
    /// - Stored procedure must return a result set (SELECT statement)
    /// - Use SqlParameter for all input parameters
    ///
    /// Example:
    /// <code>
    /// var initiatives = await spRepo.ExecuteStoredProcedureAsync&lt;Initiative&gt;(
    ///     "[dbo].[SP_GetInitiativesByProgram]",
    ///     new SqlParameter("@ProgramId", programId),
    ///     new SqlParameter("@IncludeInactive", false)
    /// );
    /// </code>
    /// </remarks>
    Task<List<TEntity>> ExecuteStoredProcedureAsync<TEntity>(string storedProcedureName, params object[] parameters)
        where TEntity : class;

    /// <summary>
    /// Executes a stored procedure that returns a single scalar value.
    /// </summary>
    /// <typeparam name="T">The type of the scalar result (e.g., int, decimal, string).</typeparam>
    /// <param name="storedProcedureName">The name of the stored procedure.</param>
    /// <param name="parameters">SQL parameters for the stored procedure.</param>
    /// <returns>The scalar value returned by the stored procedure.</returns>
    /// <remarks>
    /// Use cases:
    /// - COUNT queries returning total counts
    /// - SUM/AVG/MIN/MAX aggregations
    /// - Single value lookups
    ///
    /// Example:
    /// <code>
    /// var totalBudget = await spRepo.ExecuteScalarAsync&lt;decimal&gt;(
    ///     "[dbo].[SP_GetTotalProgramBudget]",
    ///     new SqlParameter("@PillarId", pillarId)
    /// );
    /// </code>
    /// </remarks>
    Task<T?> ExecuteScalarAsync<T>(string storedProcedureName, params object[] parameters);

    /// <summary>
    /// Executes a stored procedure that performs data modifications (INSERT, UPDATE, DELETE).
    /// Does not return a result set.
    /// </summary>
    /// <param name="storedProcedureName">The name of the stored procedure.</param>
    /// <param name="parameters">SQL parameters for the stored procedure.</param>
    /// <returns>The number of rows affected by the stored procedure.</returns>
    /// <remarks>
    /// Use cases:
    /// - Bulk updates affecting multiple tables
    /// - Complex business logic requiring multiple operations
    /// - Data cleanup and maintenance procedures
    ///
    /// Example:
    /// <code>
    /// int rowsAffected = await spRepo.ExecuteNonQueryAsync(
    ///     "[dbo].[SP_UpdateInitiativeStatus]",
    ///     new SqlParameter("@InitiativeId", initiativeId),
    ///     new SqlParameter("@NewStatusId", statusId),
    ///     new SqlParameter("@UpdatedBy", currentUserId)
    /// );
    /// </code>
    /// </remarks>
    Task<int> ExecuteNonQueryAsync(string storedProcedureName, params object[] parameters);

    /// <summary>
    /// Executes a raw SQL query that returns a collection of entities.
    /// </summary>
    /// <typeparam name="TEntity">The entity type to map results to. Must be a class.</typeparam>
    /// <param name="sql">The raw SQL query. Use {0}, {1}, etc. for parameter placeholders.</param>
    /// <param name="parameters">SQL parameters for the query. Use SqlParameter to prevent SQL injection.</param>
    /// <returns>A list of entities mapped from the query result set.</returns>
    /// <remarks>
    /// Use cases:
    /// - Complex queries not supported by LINQ
    /// - Performance-critical queries requiring specific SQL
    /// - Queries using database-specific features
    ///
    /// WARNING: Always use parameterized queries. Never concatenate user input into SQL strings.
    ///
    /// Example:
    /// <code>
    /// var sql = @"
    ///     SELECT i.*
    ///     FROM Initiatives i
    ///     INNER JOIN VisionPrograms vp ON i.ProgramId = vp.Id
    ///     WHERE vp.PillarId = {0}
    ///     AND i.CompletionPercentage >= {1}
    ///     ORDER BY i.Priority DESC";
    ///
    /// var initiatives = await spRepo.ExecuteRawSqlAsync&lt;Initiative&gt;(
    ///     sql,
    ///     new SqlParameter { Value = pillarId },
    ///     new SqlParameter { Value = 75 }
    /// );
    /// </code>
    /// </remarks>
    Task<List<TEntity>> ExecuteRawSqlAsync<TEntity>(string sql, params object[] parameters)
        where TEntity : class;

    /// <summary>
    /// Executes a raw SQL command that performs data modifications (INSERT, UPDATE, DELETE).
    /// Does not return a result set.
    /// </summary>
    /// <param name="sql">The raw SQL command. Use {0}, {1}, etc. for parameter placeholders.</param>
    /// <param name="parameters">SQL parameters for the command. Use SqlParameter to prevent SQL injection.</param>
    /// <returns>The number of rows affected by the SQL command.</returns>
    /// <remarks>
    /// Use cases:
    /// - Bulk updates with complex conditions
    /// - Data migrations and cleanup
    /// - Performance-critical operations
    ///
    /// WARNING: Always use parameterized queries. Never concatenate user input into SQL strings.
    ///
    /// Example:
    /// <code>
    /// var sql = @"
    ///     UPDATE Initiatives
    ///     SET StatusId = {0}, UpdatedBy = {1}, UpdatedOn = GETUTCDATE()
    ///     WHERE ProgramId = {2} AND CompletionPercentage = 100 AND StatusId != {0}";
    ///
    /// int rowsAffected = await spRepo.ExecuteRawSqlCommandAsync(
    ///     sql,
    ///     new SqlParameter { Value = completedStatusId },
    ///     new SqlParameter { Value = currentUserId },
    ///     new SqlParameter { Value = programId }
    /// );
    /// </code>
    /// </remarks>
    Task<int> ExecuteRawSqlCommandAsync(string sql, params object[] parameters);
}
