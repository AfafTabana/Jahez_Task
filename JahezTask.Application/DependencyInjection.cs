
using JahezTask.Application.Interfaces;
using JahezTask.Application.Interfaces.Services;
using JahezTask.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JahezTask.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
        this IServiceCollection services)
        {
            // Services
          
            services.AddScoped<INotificationReminderService, NotificationReminderService>();

            // AutoMapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            //mediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));  

            //usercontext 
            services.AddScoped<IUserContext, UserContext>();

            return services;
        }
    }
}
