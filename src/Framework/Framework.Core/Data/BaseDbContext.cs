using Microsoft.EntityFrameworkCore;

namespace Framework.Core.Data;

/// <summary>
/// Abstract base class for all database contexts in the application.
/// Provides automatic audit tracking and common database operations.
/// </summary>
/// <typeparam name="TContext">The derived DbContext type (for proper type resolution).</typeparam>
/// <remarks>
/// Design Purpose:
/// - Centralized audit field management (CreatedBy, UpdatedBy, etc.)
/// - Automatic timestamp tracking
/// - Current user tracking via HttpContext
/// - Base implementation of IBaseDbContext
/// - Consistent behavior across all database contexts
///
/// Key Features:
/// - Automatic audit field population on SaveChanges
/// - Support for FullAuditedEntityBase entities
/// - Configurable current user provider
/// - Inherits from EF Core DbContext
///
/// Usage Example:
/// <code>
/// // In Infrastructure layer
/// public class AppDbContext : BaseDbContext&lt;AppDbContext&gt;, IAppDbContext
/// {
///     public AppDbContext(DbContextOptions&lt;AppDbContext&gt; options) : base(options)
///     {
///     }
///
///     public DbSet&lt;StrategicObjective&gt; StrategicObjectives { get; set; }
///     public DbSet&lt;Initiative&gt; Initiatives { get; set; }
///
///     protected override void OnModelCreating(ModelBuilder modelBuilder)
///     {
///         // Auto-discovery logic here
///         base.OnModelCreating(modelBuilder);
///     }
/// }
/// </code>
///
/// Audit Tracking:
/// - CreatedBy/CreatedOn: Set when entity state is Added
/// - UpdatedBy/UpdatedOn: Set when entity state is Modified
/// - Timestamps use UTC to avoid timezone issues
/// - Current user obtained from HttpContext (or "System" if not available)
///
/// Current User Resolution:
/// - In API context: Uses HttpContext.User.Identity.Name
/// - In background jobs: Uses "System" or configured service account
/// - Can be overridden by setting CurrentUserName property
/// </remarks>
public abstract class BaseDbContext<TContext> : DbContext, IBaseDbContext
    where TContext : DbContext
{
    /// <summary>
    /// Gets or sets the current user's username for audit tracking.
    /// If null, attempts to resolve from HttpContext or uses "System" as fallback.
    /// </summary>
    /// <remarks>
    /// Set this property manually for:
    /// - Background jobs running outside HTTP context
    /// - Console applications or utilities
    /// - Testing scenarios requiring specific user names
    ///
    /// Example:
    /// <code>
    /// // In Hangfire job
    /// _dbContext.CurrentUserName = "BackgroundJob:KPICalculation";
    /// await _dbContext.SaveChangesAsync();
    /// </code>
    /// </remarks>
    public string? CurrentUserName { get; set; }

    /// <summary>
    /// Initializes a new instance of the BaseDbContext with the specified options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected BaseDbContext(DbContextOptions<TContext> options) : base(options)
    {
    }

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// Automatically updates audit fields (CreatedBy, UpdatedBy, etc.) before saving.
    /// </summary>
    /// <returns>The number of state entries written to the database.</returns>
    /// <remarks>
    /// Audit Field Updates:
    /// - Added entities: Sets CreatedBy and CreatedOn
    /// - Modified entities: Sets UpdatedBy and UpdatedOn
    /// - Deleted entities: No audit updates (hard delete)
    ///
    /// All timestamps use DateTime.UtcNow for consistency across timezones.
    /// </remarks>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Asynchronously saves all changes made in this context to the database.
    /// Automatically updates audit fields (CreatedBy, UpdatedBy, etc.) before saving.
    /// </summary>
    /// <param name="cancellationToken">A token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates audit fields for all tracked entities that inherit from FullAuditedEntityBase.
    /// Called automatically by SaveChanges and SaveChangesAsync.
    /// </summary>
    /// <remarks>
    /// Process:
    /// 1. Get all tracked entities from ChangeTracker
    /// 2. Filter entities inheriting from FullAuditedEntityBase
    /// 3. For Added entities: Set CreatedBy and CreatedOn
    /// 4. For Modified entities: Set UpdatedBy and UpdatedOn
    ///
    /// Current User Resolution Order:
    /// 1. CurrentUserName property (if set manually)
    /// 2. HttpContext.User.Identity.Name (if available via dependency injection)
    /// 3. "System" (fallback for background jobs, console apps, etc.)
    /// </remarks>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is FullAuditedEntityBase<int> || e.Entity is FullAuditedEntityBase<Guid> || e.Entity is FullAuditedEntityBase<long>)
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        var currentUser = GetCurrentUserName();
        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                // Set creation audit fields
                if (entry.Entity is FullAuditedEntityBase<int> entity1)
                {
                    entity1.CreatedBy = currentUser;
                    entity1.CreatedOn = now;
                }
                else if (entry.Entity is FullAuditedEntityBase<Guid> entity2)
                {
                    entity2.CreatedBy = currentUser;
                    entity2.CreatedOn = now;
                }
                else if (entry.Entity is FullAuditedEntityBase<long> entity3)
                {
                    entity3.CreatedBy = currentUser;
                    entity3.CreatedOn = now;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                // Set modification audit fields
                if (entry.Entity is FullAuditedEntityBase<int> entity1)
                {
                    entity1.UpdatedBy = currentUser;
                    entity1.UpdatedOn = now;
                }
                else if (entry.Entity is FullAuditedEntityBase<Guid> entity2)
                {
                    entity2.UpdatedBy = currentUser;
                    entity2.UpdatedOn = now;
                }
                else if (entry.Entity is FullAuditedEntityBase<long> entity3)
                {
                    entity3.UpdatedBy = currentUser;
                    entity3.UpdatedOn = now;
                }
            }
        }
    }

    /// <summary>
    /// Gets the current user's username for audit tracking.
    /// </summary>
    /// <returns>The current username or "System" if unavailable.</returns>
    /// <remarks>
    /// Resolution Order:
    /// 1. CurrentUserName property (if set)
    /// 2. "System" (fallback)
    ///
    /// NOTE: HttpContext resolution will be added when HttpContextAccessor is configured in Program.cs
    /// For now, using "System" as default until dependency injection is fully configured in future stories.
    /// </remarks>
    protected virtual string GetCurrentUserName()
    {
        // Return manually set username if available
        if (!string.IsNullOrEmpty(CurrentUserName))
        {
            return CurrentUserName;
        }

        // TODO: In future stories, inject IHttpContextAccessor and resolve from HttpContext
        // Example:
        // if (_httpContextAccessor?.HttpContext?.User?.Identity?.Name != null)
        // {
        //     return _httpContextAccessor.HttpContext.User.Identity.Name;
        // }

        // Default fallback for background jobs, console apps, or before DI is configured
        return "System";
    }
}
