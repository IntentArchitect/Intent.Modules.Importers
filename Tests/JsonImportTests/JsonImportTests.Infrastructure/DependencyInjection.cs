using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Common.Interfaces;
using JsonImportTests.Domain.Repositories;
using JsonImportTests.Infrastructure.Configuration;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace JsonImportTests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCosmosRepository();
            services.AddScoped<ICustomerRepository, customerCosmosDBRepository>();
            services.AddScoped<IInvoiceRepository, invoiceCosmosDBRepository>();
            services.AddScoped<CosmosDBUnitOfWork>();
            services.AddScoped<ICosmosDBUnitOfWork>(provider => provider.GetRequiredService<CosmosDBUnitOfWork>());
            services.AddMassTransitConfiguration(configuration);
            return services;
        }
    }
}