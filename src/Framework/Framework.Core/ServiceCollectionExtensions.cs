using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core;

/// <summary>
/// Extension methods for registering Framework.Core shared services.
/// Configures cross-cutting concerns and shared infrastructure services.
/// </summary>
/// <remarks>
/// This class provides dependency injection configuration for:
/// - Shared services used across multiple applications
/// - Cross-cutting concerns (caching, notifications, background jobs)
/// - Utility services (file handling, QR code generation, etc.)
/// - Common database contexts (Commons, Identity)
///
/// Usage in Program.cs:
/// <code>
/// // In Program.cs after builder.Services configuration
/// builder.Services.ConfigureSharedApplicationServices(commonsConnectionString);
/// </code>
///
/// Design Purpose:
/// - Centralize registration of framework-level services
/// - Provide reusable infrastructure components
/// - Reduce duplication across multiple applications
/// - Enable consistent configuration across projects
///
/// Current State (Story 0.0):
/// - Placeholder implementation - will be fully implemented in future stories
/// - Core services will be added as they are implemented
/// - This foundation enables future applications to reuse these services
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures Framework.Core shared services for dependency injection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <param name="commonsConnectionString">The connection string for the shared Commons database.</param>
    /// <returns>The IServiceCollection for method chaining.</returns>
    /// <remarks>
    /// This method will be implemented in future stories to register:
    ///
    /// **Database Contexts (Future Stories):**
    /// - CommonsDbContext (shared lookup tables, logs, notifications)
    /// - IdentityDbContext (user management, roles, permissions)
    ///
    /// **Caching Services (Future Stories):**
    /// <code>
    /// // Memory cache for lookup data
    /// services.AddMemoryCache();
    ///
    /// // Distributed cache for multi-server scenarios
    /// services.AddStackExchangeRedisCache(options =>
    /// {
    ///     options.Configuration = configuration["Redis:ConnectionString"];
    /// });
    /// </code>
    ///
    /// **Notification Services (Future Stories):**
    /// - IEmailService → EmailService (SMTP)
    /// - ISMSService → SMSService (SMS gateway)
    /// - IPushNotificationService → FirebaseService
    /// - IInAppNotificationService → SignalRNotificationService
    ///
    /// **Background Job Services (Future Stories):**
    /// <code>
    /// // Hangfire for background jobs
    /// services.AddHangfire(config =>
    /// {
    ///     config.UseSqlServerStorage(commonsConnectionString);
    /// });
    ///
    /// services.AddHangfireServer();
    /// </code>
    ///
    /// **File Handling Services (Future Stories):**
    /// - IFileStorageService → AzureBlobStorageService or LocalFileService
    /// - IExcelService → ExcelJsService (using EPPlus/ExcelDataReader)
    /// - IPdfService → PdfService (using iText/Select.HtmlToPdf)
    /// - IQRCodeService → QRCodeService (using ZXing.Net)
    ///
    /// **Validation Services (Future Stories):**
    /// - IPhoneNumberValidator → LibPhoneNumberValidator
    /// - INationalIdValidator → SaudiNationalIdValidator
    /// - IIbanValidator → IbanValidator
    ///
    /// **Localization Services (Future Stories):**
    /// <code>
    /// // Configure request localization
    /// services.Configure&lt;RequestLocalizationOptions&gt;(options =>
    /// {
    ///     var supportedCultures = new[] { "ar-SA", "en-US" };
    ///     options.SetDefaultCulture("ar-SA")
    ///         .AddSupportedCultures(supportedCultures)
    ///         .AddSupportedUICultures(supportedCultures);
    /// });
    /// </code>
    ///
    /// **Middleware Configuration (Future Stories):**
    /// - Exception handling middleware
    /// - Request/response logging middleware
    /// - Performance monitoring middleware
    /// - API versioning middleware
    ///
    /// Lifetime Scopes:
    /// - DbContexts: Scoped (per request)
    /// - Caching: Singleton (shared across requests)
    /// - Notification Services: Scoped (may have per-request dependencies)
    /// - File Services: Scoped or Singleton (depending on statelessness)
    /// - Background Jobs: Singleton (Hangfire manages lifecycle)
    /// </remarks>
    public static IServiceCollection ConfigureSharedApplicationServices(
        this IServiceCollection services,
        string commonsConnectionString)
    {
        // TODO: Implement service registration in future stories

        // Future implementations will add:
        // 1. Commons and Identity DbContext registration
        // 2. Caching service registration (Memory + Distributed)
        // 3. Notification service registration (Email, SMS, Push, In-App)
        // 4. Hangfire background job registration
        // 5. File handling service registration (Storage, Excel, PDF, QR)
        // 6. Validation service registration (Phone, NationalId, IBAN)
        // 7. Localization service registration
        // 8. Middleware registration

        return services;
    }
}
