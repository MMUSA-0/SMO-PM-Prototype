using Framework.Core.Caching;
using Framework.Core.Data.Repositories;
using Framework.Core.Data.Uow;
using Framework.Core.Drawing;
using Framework.Core.Notifications;
using Framework.Core.SharedServices.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Framework.Core.SharedServices.Services
{
    public static class CommonStartup
    {
        public static void ConfigureSharedApplicationServices(this IServiceCollection services, string connectionString)
        {
            services.TryAddScoped<ICacheManager, MemoryCacheManager>();

            services.AddDbContext<CommonsDbContext>(options => options.UseSqlServer(connectionString));

            services.AddLocalization();
            services.TryAddScoped(typeof(IUnitOfWorkBase<>), typeof(UnitOfWorkBase<>));
            services.TryAddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
            services.TryAddScoped<ICommonsDbContext, CommonsDbContext>();

            services.AddScoped<INotificationsManager, NotificationsManager>();
            //services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<NotificationTemplateService>();
            services.AddScoped<AppSettingsService>();
            services.AddScoped<PDFGeneratorAppService>();
            services.AddScoped<LogAppService>();
            services.AddScoped<ILoggingService, LoggingService>();


            services.TryAddScoped<IEmailService, SmtpEmailService>();
            
            services.TryAddScoped<AttachmentService, AttachmentService>();
            services.TryAddScoped<AttachmentHelperAppService, AttachmentHelperAppService>();
            services.TryAddScoped<PDFHelper, PDFHelper>();

            // Caching
            services.AddMemoryCache();
            services.AddResponseCompression();
        }

        public static void UseSharedCommonDBMigration(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
            .CreateScope();
            serviceScope.ServiceProvider.GetService<CommonsDbContext>().Database.Migrate();
        }


    }
}
