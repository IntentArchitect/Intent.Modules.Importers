using System.Reflection;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestDataGenerator.Application.Implementation;
using TestDataGenerator.Application.Interfaces;
using TestDataGenerator.Domain.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestDataGenerator.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<IOrderDomainService, OrderDomainService>();
            services.AddTransient<IOrdersService, OrdersService>();
            services.AddTransient<IProductsService, ProductsService>();
            return services;
        }
    }
}