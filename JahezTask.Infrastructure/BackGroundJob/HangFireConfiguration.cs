
using Microsoft.Extensions.Configuration;
using Hangfire;
using Hangfire.SqlServer;
using JahezTask.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Infrastructure.BackGroundJob
{
    public static class HangFireConfiguration
    {
        public static IServiceCollection AddHangfireConfiguration(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Configure Hangfire to use SQL Server
            services.AddHangfire(config =>
            {
                // Get connection string
                var connectionString = configuration.GetConnectionString("DefaultConnection");

                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true,
                        SchemaName = "Hangfire"
                    });
            });
            services.AddHangfireServer();
            

    

            return services;
        }


    }
}
