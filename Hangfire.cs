using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Hangfire;
using Hangfire.SqlServer;


namespace MessageScheduler
{
    internal static class Hangfire
    {
        public static void ConfigureService(string connectionString, IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            services.AddHangfireServer();
        }

        public static void Configure(IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHangfireDashboard();
            });
            app.UseHangfireDashboard();
        }

        public static void InitializeJobs()
        {
            RecurringJob.AddOrUpdate<Workers.SendTextJob>(job => job.Execute(), Cron.Minutely);
        }
    }
}
