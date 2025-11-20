using JahezTask.Domain.Entities;
using JahezTask.Persistence.Data;
using Microsoft.Extensions.DependencyInjection;
using System;

using Microsoft.AspNetCore.Identity;

namespace JahezTask.Infrastructure.Identity
{
    public static  class IdentityConfiguration
    {

        public static IServiceCollection AddIdentityConfiguration(
            this IServiceCollection services)
        {

            // Configure Identity
            services.AddIdentity<ApplicationUser, IdentityRole<int>>(
           options =>
               {
                   options.Password.RequireDigit = true;
                   options.Password.RequiredLength = 6;
                   options.Password.RequireLowercase = false;
                   options.Password.RequireUppercase = false;
                   options.Password.RequireNonAlphanumeric = false;

               }
                )
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            return services;
        }

}
}
