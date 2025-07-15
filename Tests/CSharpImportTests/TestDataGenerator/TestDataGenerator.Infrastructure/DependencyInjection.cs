using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestDataGenerator.Application.Common.Interfaces;
using TestDataGenerator.Domain.Common.Interfaces;
using TestDataGenerator.Domain.Repositories;
using TestDataGenerator.Domain.Repositories.RepoTest;
using TestDataGenerator.Infrastructure.Persistence;
using TestDataGenerator.Infrastructure.Repositories;
using TestDataGenerator.Infrastructure.Repositories.RepoTest;
using TestDataGenerator.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace TestDataGenerator.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IAggregateRoot1Repository, AggregateRoot1Repository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}