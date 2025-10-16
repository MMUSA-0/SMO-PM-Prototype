using Microsoft.Extensions.DependencyInjection;

namespace SMO.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services.
/// Configures data access, repositories, and external API integrations.
/// </summary>
/// <remarks>
/// This class provides dependency injection configuration for:
/// - Database contexts (AppDbContext, IdentityContext, CommonsContext)
/// - Repository implementations (generic and specialized)
/// - Unit of Work pattern
/// - External API clients (Nafath, SMS, ADAA, GaStat)
/// - Background job services
///
/// Usage in Program.cs:
/// <code>
/// // In Program.cs after builder.Services configuration
/// builder.Services.ConfigureInfrastructureServices(connectionString);
/// </code>
///
/// Auto-Registration Pattern:
/// - This extension method will use reflection to auto-discover and register:
///   * All IRepository implementations
///   * All specialized repository interfaces and implementations
///   * All API client implementations
///
/// Current State (Story 0.0):
/// - Placeholder implementation - will be fully implemented in future stories
/// - Basic registrations (AppDbContext, IUnitOfWork, IRepository) are already in Program.cs
/// - Auto-registration logic will be added when entities and specialized repositories are created
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures Infrastructure layer services for dependency injection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="connectionString">The database connection string for AppDbContext.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <remarks>
    /// This method will be implemented in future stories to register:
    ///
    /// **Data Access:**
    /// - AppDbContext (already registered in Program.cs)
    /// - IAppDbContext (already registered in Program.cs)
    /// - IUnitOfWork → UnitOfWork (already registered in Program.cs)
    /// - IRepository&lt;&gt; → Repository&lt;&gt; (already registered in Program.cs)
    ///
    /// **Specialized Repositories (Future Stories):**
    /// - IPillarRepository → PillarRepository
    /// - IStrategicObjectiveRepository → StrategicObjectiveRepository
    /// - IInitiativeRepository → InitiativeRepository
    /// - IKPIRepository → KPIRepository
    /// - And many more specialized repositories...
    ///
    /// **External API Clients (Future Stories):**
    /// - INafathService → NafathServiceProxy
    /// - ISMSService → SMSServiceProxy
    /// - IAdaaService → AdaaServiceProxy
    /// - IGaStatService → GaStatServiceProxy
    ///
    /// **Auto-Discovery Pattern (Future Implementation):**
    /// <code>
    /// // Example: Auto-register all repository implementations
    /// var repositoryTypes = Assembly.GetExecutingAssembly().GetTypes()
    ///     .Where(t => t.Name.EndsWith("Repository") && !t.IsAbstract && !t.IsInterface);
    ///
    /// foreach (var type in repositoryTypes)
    /// {
    ///     var interfaceType = type.GetInterfaces()
    ///         .FirstOrDefault(i => i.Name == $"I{type.Name}");
    ///
    ///     if (interfaceType != null)
    ///     {
    ///         services.AddScoped(interfaceType, type);
    ///     }
    /// }
    /// </code>
    ///
    /// Lifetime Scopes:
    /// - DbContext: Scoped (per request)
    /// - Repositories: Scoped (per request)
    /// - UnitOfWork: Scoped (per request)
    /// - API Clients: Scoped or Singleton (depending on statelessness)
    /// </remarks>
    public static IServiceCollection ConfigureInfrastructureServices(
        this IServiceCollection services,
        string connectionString)
    {
        // TODO: Implement service registration in future stories

        // Basic registrations are already handled in Program.cs:
        // - AddDbContext<AppDbContext>
        // - AddScoped<IAppDbContext>
        // - AddScoped<IUnitOfWork, UnitOfWork>
        // - AddScoped(typeof(IRepository<>), typeof(Repository<>))

        // Future implementations will add:
        // 1. Specialized repository registrations (auto-discovery)
        // 2. External API client registrations
        // 3. Background job service registrations
        // 4. Cache service registrations

        return services;
    }
}
