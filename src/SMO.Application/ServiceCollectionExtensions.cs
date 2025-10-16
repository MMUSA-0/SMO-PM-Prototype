using Microsoft.Extensions.DependencyInjection;

namespace SMO.Application;

/// <summary>
/// Extension methods for registering Application layer services.
/// Configures business logic services, AutoMapper profiles, and validation.
/// </summary>
/// <remarks>
/// This class provides dependency injection configuration for:
/// - Application services (business logic layer)
/// - AutoMapper profiles for DTO mapping
/// - FluentValidation validators
/// - Domain event handlers (if implemented)
///
/// Usage in Program.cs:
/// <code>
/// // In Program.cs after builder.Services configuration
/// builder.Services.ConfigureApplicationServices();
/// </code>
///
/// Auto-Registration Pattern:
/// - This extension method will use reflection to auto-discover and register:
///   * All application service interfaces and implementations
///   * All AutoMapper profiles
///   * All FluentValidation validators
///
/// Current State (Story 0.0):
/// - Placeholder implementation - will be fully implemented in future stories
/// - Services will be added as features are implemented in subsequent stories
/// - Auto-registration logic ensures new services are automatically discovered
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures Application layer services for dependency injection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <remarks>
    /// This method will be implemented in future stories to register:
    ///
    /// **Application Services (Future Stories):**
    /// - IPillarAppService → PillarAppService
    /// - IThemeAppService → ThemeAppService
    /// - IStrategicObjectiveAppService → StrategicObjectiveAppService
    /// - IVisionProgramAppService → VisionProgramAppService
    /// - IInitiativeAppService → InitiativeAppService
    /// - IKPIAppService → KPIAppService
    /// - IDashboardAppService → DashboardAppService
    /// - IAttachmentAppService → AttachmentAppService
    /// - IWorkflowAppService → WorkflowAppService
    /// - And many more application services...
    ///
    /// **AutoMapper Configuration (Future Stories):**
    /// <code>
    /// // Register all AutoMapper profiles from this assembly
    /// services.AddAutoMapper(Assembly.GetExecutingAssembly());
    ///
    /// // Profiles to be created in future stories:
    /// // - ApplicationAutoMappingProfile (base profile)
    /// // - PillarMappingProfile
    /// // - StrategicObjectiveMappingProfile
    /// // - InitiativeMappingProfile
    /// // - KPIMappingProfile
    /// // And more...
    /// </code>
    ///
    /// **FluentValidation Configuration (Future Stories):**
    /// <code>
    /// // Register all validators from this assembly
    /// services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    ///
    /// // Validators to be created in future stories:
    /// // - CreateStrategicObjectiveValidator
    /// // - UpdateInitiativeValidator
    /// // - CreateKPIValidator
    /// // And more...
    /// </code>
    ///
    /// **Auto-Discovery Pattern (Future Implementation):**
    /// <code>
    /// // Example: Auto-register all application service implementations
    /// var serviceTypes = Assembly.GetExecutingAssembly().GetTypes()
    ///     .Where(t => t.Name.EndsWith("AppService") && !t.IsAbstract && !t.IsInterface);
    ///
    /// foreach (var type in serviceTypes)
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
    /// - Application Services: Scoped (per request)
    /// - AutoMapper: Singleton (stateless)
    /// - Validators: Scoped (may have dependencies)
    /// </remarks>
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
    {
        // TODO: Implement service registration in future stories

        // Future implementations will add:
        // 1. Application service registrations (auto-discovery by naming convention)
        // 2. AutoMapper profile registration (all profiles in assembly)
        // 3. FluentValidation validator registration (all validators in assembly)
        // 4. Domain event handler registration (if implemented)

        return services;
    }
}
