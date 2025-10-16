using Hangfire.SqlServer;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire.Logging.LogProviders;

namespace Framework.Core.BackgroundJobs
{
    public static class HangfireExtensions
    {
        /// <summary>
        /// Initializes the hang fire services.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static IServiceCollection InitHangFireServices(this IServiceCollection services,
                                                               string connectionString)
        {
            var sqlStorage = new SqlServerStorage(connectionString);

            JobStorage.Current = sqlStorage;

            services.AddHangfire(config =>
            {
                config.UseLogProvider(new ColouredConsoleLogProvider());

                GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString,
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    });
            });

            services.AddHangfireServer();

            var serviceProvider = services.BuildServiceProvider();
            GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(serviceProvider));

            var backGroundTasks = serviceProvider.GetService<IBackgroundTasks>();
            backGroundTasks?.Init();
            return services;
        }

        /// <summary>
        /// Uses the imo hangfire dashboard.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="onUrl">The on URL.</param>
        /// <returns></returns>
        public static void InitHangfireDashboard(this IApplicationBuilder app,
                                                string onUrl = "/back-jobs")
        {
            var options = new BackgroundJobServerOptions
            {
#if DEBUG
                Queues = [Environment.MachineName.ToLower(), "default"],
#endif
                WorkerCount = Environment.ProcessorCount * 1
            };

            app.UseHangfireServer(options);

#if DEBUG
            //app.UseHangfireDashboard(onUrl);

            //app.UseHangfireDashboard(
            //  onUrl,
            //  new DashboardOptions { Authorization = new[] { new HangFireDashboardAuthenticationFilter() } });
#endif

        app.UseHangfireDashboard(
        onUrl,
        new DashboardOptions { Authorization = new[] { new HangfireDashboardAuthFilter() } });
        }


    }
}
