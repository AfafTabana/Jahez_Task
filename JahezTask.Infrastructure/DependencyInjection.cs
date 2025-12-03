using JahezTask.Application.Interfaces;
using JahezTask.Infrastructure.BackGroundJob;
using JahezTask.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Identity
            services.AddIdentityConfiguration();

            // Hangfire
            services.AddHangfireConfiguration(configuration);

           

            return services;
        }
    }
}
