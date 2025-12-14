using System.Reflection;
using Framework.Core.Data;
using Framework.Core.Data.Mapping;
using Microsoft.EntityFrameworkCore;
using SMO.Domain.Interfaces;
using SMO.Domain.Entities.Core;
using SMO.Domain.Entities.Performance;
using SMO.Domain.Entities.Risk;
using SMO.Domain.Entities.Shared;
using SMO.Domain.Entities.EmployeePerformance;

namespace SMO.Infrastructure.Data;

/// <summary>
/// Main application database context for the SMO Vision Center.
/// Implements Clean Architecture data access with automatic entity configuration discovery.
/// </summary>
/// <remarks>
/// Architecture:
/// - Inherits from BaseDbContext for audit tracking
/// - Implements IAppDbContext for dependency inversion
/// - Uses auto-discovery pattern for entity configurations
/// - Applies global query filters for soft deletion
///
/// Key Features:
/// 1. Auto-Discovery Pattern:
///    - Scans assembly for EntityTypeConfiguration classes
///    - Automatically applies all entity mappings
///    - Scans assembly for EnumMapping classes
///    - No need to manually register configurations
///
/// 2. Global Query Filters:
///    - Automatically filters IsActive=true for all LookupEntityBase entities
///    - Reduces code duplication in repositories
///    - Can be bypassed with IgnoreQueryFilters() when needed
///
/// 3. Audit Tracking:
///    - Inherited from BaseDbContext
///    - Automatically sets CreatedBy, UpdatedBy, CreatedOn, UpdatedOn
///    - Uses HttpContext for current user (or "System" fallback)
///
/// Usage Example:
/// <code>
/// // In Program.cs (will be configured in Task 22)
/// builder.Services.AddDbContext&lt;AppDbContext&gt;(options =>
///     options.UseSqlServer(connectionString));
///
/// builder.Services.AddScoped&lt;IAppDbContext&gt;(provider =>
///     provider.GetRequiredService&lt;AppDbContext&gt;());
/// </code>
///
/// Entity Configuration Example:
/// <code>
/// // Create in SMO.Infrastructure/Mapping/Vision/PillarMapping.cs
/// public class PillarMapping : EntityTypeConfiguration&lt;Pillar&gt;
/// {
///     public override void Configure(EntityTypeBuilder&lt;Pillar&gt; builder)
///     {
///         builder.ToTable("Pillars");
///         builder.HasKey(x => x.Id);
///         // ... more configuration
///     }
/// }
/// // This mapping will be automatically discovered and applied
/// </code>
///
/// Future Enhancements (Story 1.x+):
/// - DbSet properties will be added as entities are created
/// - Entity configurations will be added in Mapping/ folder
/// - Enum mappings will be added for lookup tables
/// - Migrations will be generated and applied
/// </remarks>
public class AppDbContext : BaseDbContext<AppDbContext>, IAppDbContext
{
    /// <summary>
    /// Initializes a new instance of AppDbContext with the specified options.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures the model using Fluent API and auto-discovery pattern.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    /// <remarks>
    /// Configuration Steps:
    /// 1. Auto-discover and apply EntityTypeConfiguration classes
    /// 2. Auto-discover and apply EnumMapping classes
    /// 3. Apply global query filter for IsActive on LookupEntityBase entities
    /// 4. Call base.OnModelCreating for any framework-level configurations
    ///
    /// Auto-Discovery Pattern Benefits:
    /// - No manual registration of entity configurations
    /// - Configurations automatically found via reflection
    /// - New entities automatically picked up when added
    /// - Reduces maintenance overhead
    ///
    /// Performance Note:
    /// - Reflection runs only once at application startup
    /// - Minimal performance impact (milliseconds)
    /// - Results are cached by EF Core
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Apply Configurations from Configuration Files
        
        // Apply all configurations from the Configurations folder
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        #endregion
        
        #region Auto-Discover EntityTypeConfiguration Classes

        // Find all EntityTypeConfiguration<TEntity> derived classes in this assembly
        var typeConfigurations = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => (type.BaseType?.IsGenericType ?? false) &&
                          type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>))
            .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition);

        // Instantiate and apply each configuration
        foreach (var typeConfiguration in typeConfigurations)
        {
            var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration)!;
            configuration.ApplyConfiguration(modelBuilder);
        }

        #endregion

        #region Auto-Discover EnumMapping Classes

        // Find all EnumMapping<TEnum> derived classes in this assembly
        var enumConfigurations = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => (type.BaseType?.IsGenericType ?? false) &&
                          type.BaseType.GetGenericTypeDefinition() == typeof(EntityTypeConfiguration<>))
            .Where(type => !type.IsAbstract && !type.IsGenericTypeDefinition);

        // Instantiate and apply each enum mapping
        foreach (var typeConfiguration in enumConfigurations)
        {
            var configuration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration)!;
            configuration.ApplyConfiguration(modelBuilder);
        }

        #endregion

        #region Apply Global Query Filter for IsActive

        // Apply global query filter to all LookupEntityBase entities
        // This ensures that only active lookup records are returned by default
        // Use IgnoreQueryFilters() in queries to include inactive records
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // Check if entity type inherits from LookupEntityBase (any TKey variant)
            if (typeof(LookupEntityBase<int>).IsAssignableFrom(entityType.ClrType) ||
                typeof(LookupEntityBase<Guid>).IsAssignableFrom(entityType.ClrType) ||
                typeof(LookupEntityBase<long>).IsAssignableFrom(entityType.ClrType))
            {
                // Use reflection to call SetActiveFilter<TEntity>
                var method = typeof(AppDbContext)
                    .GetMethod(nameof(SetActiveFilter), BindingFlags.NonPublic | BindingFlags.Static)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(null, new object[] { modelBuilder });
            }
        }

        #endregion

        // Call base implementation for any framework-level configurations
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Configures global query filter for IsActive property on lookup entities.
    /// Called via reflection for each LookupEntityBase entity type.
    /// </summary>
    /// <typeparam name="TEntity">The lookup entity type.</typeparam>
    /// <param name="builder">The model builder.</param>
    /// <remarks>
    /// This filter is applied globally to all queries against lookup entities.
    /// To include inactive records, use: query.IgnoreQueryFilters()
    ///
    /// Example:
    /// <code>
    /// // Normal query - only active records
    /// var activeStatuses = await context.Set&lt;InitiativeStatus&gt;().ToListAsync();
    ///
    /// // Include inactive records
    /// var allStatuses = await context.Set&lt;InitiativeStatus&gt;()
    ///     .IgnoreQueryFilters()
    ///     .ToListAsync();
    /// </code>
    /// </remarks>
    private static void SetActiveFilter<TEntity>(ModelBuilder builder)
        where TEntity : class
    {
        // Check which TKey variant this entity uses and cast accordingly
        if (typeof(LookupEntityBase<int>).IsAssignableFrom(typeof(TEntity)))
        {
            builder.Entity<TEntity>().HasQueryFilter(e => ((LookupEntityBase<int>)(object)e).IsActive);
        }
        else if (typeof(LookupEntityBase<Guid>).IsAssignableFrom(typeof(TEntity)))
        {
            builder.Entity<TEntity>().HasQueryFilter(e => ((LookupEntityBase<Guid>)(object)e).IsActive);
        }
        else if (typeof(LookupEntityBase<long>).IsAssignableFrom(typeof(TEntity)))
        {
            builder.Entity<TEntity>().HasQueryFilter(e => ((LookupEntityBase<long>)(object)e).IsActive);
        }
    }

    // =============================================
    // Vision 2030 Module DbSets
    // =============================================
    public DbSet<VisionProgram> VisionPrograms { get; set; }
    public DbSet<Initiative> Initiatives { get; set; }
    public DbSet<KPI> KPIs { get; set; }
    public DbSet<KPIValue> KPIValues { get; set; }
    public DbSet<KPITarget> KPITargets { get; set; }
    public DbSet<InitiativeMilestone> InitiativeMilestones { get; set; }
    
    // =============================================
    // Performance Management Module DbSets
    // =============================================
    public DbSet<Employee> Employees { get; set; }
    public DbSet<PerformanceGoal> PerformanceGoals { get; set; }
    public DbSet<PerformanceReview> PerformanceReviews { get; set; }
    public DbSet<PerformanceRating> PerformanceRatings { get; set; }
    
    // =============================================
    // Risk & Program Performance DbSets
    // =============================================
    public DbSet<ProgramRisk> ProgramRisks { get; set; }
    public DbSet<InitiativeRisk> InitiativeRisks { get; set; }
    public DbSet<ProgramAchievement> ProgramAchievements { get; set; }
    public DbSet<ProgramBudget> ProgramBudgets { get; set; }
    public DbSet<ProgramPerformance> ProgramPerformances { get; set; }
    
    // =============================================
    // Shared Resource DbSets
    // =============================================
    public DbSet<Risk> Risks { get; set; }
    public DbSet<Milestone> Milestones { get; set; }
    public DbSet<PerformanceRisk> PerformanceRisks { get; set; }
    public DbSet<PerformanceGoalMilestone> PerformanceGoalMilestones { get; set; }
    
    // NOTE: These shared entities still need to be created in Domain layer
    // public DbSet<ChangeRequest> ChangeRequests { get; set; }
    // public DbSet<SupportRequest> SupportRequests { get; set; }
    // public DbSet<Document> Documents { get; set; }
    // public DbSet<Notification> Notifications { get; set; }
    // public DbSet<AuditLog> AuditLogs { get; set; }
}
