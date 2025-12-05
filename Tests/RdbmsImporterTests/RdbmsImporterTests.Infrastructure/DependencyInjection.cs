using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RdbmsImporterTests.Application.Common.Interfaces;
using RdbmsImporterTests.Domain.Common.Interfaces;
using RdbmsImporterTests.Domain.Repositories.Dbo;
using RdbmsImporterTests.Domain.Repositories.PsgSchema2;
using RdbmsImporterTests.Domain.Repositories.PsgViews;
using RdbmsImporterTests.Domain.Repositories.Public;
using RdbmsImporterTests.Domain.Repositories.Schema2;
using RdbmsImporterTests.Domain.Repositories.Views;
using RdbmsImporterTests.Infrastructure.Persistence;
using RdbmsImporterTests.Infrastructure.Repositories.Dbo;
using RdbmsImporterTests.Infrastructure.Repositories.PsgSchema2;
using RdbmsImporterTests.Infrastructure.Repositories.PsgViews;
using RdbmsImporterTests.Infrastructure.Repositories.Public;
using RdbmsImporterTests.Infrastructure.Repositories.Schema2;
using RdbmsImporterTests.Infrastructure.Repositories.Views;
using RdbmsImporterTests.Infrastructure.Services;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PostgresAppDbContext>((sp, options) =>
            {
                options.UseNpgsql(
                    configuration.GetConnectionString("PostgresApp"),
                    b => b.MigrationsAssembly(typeof(PostgresAppDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IElementRepository, ElementRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IStoredProcedureRepository, Repositories.Dbo.StoredProcedureRepository>();
            services.AddTransient<Domain.Repositories.Public.IStoredProcedureRepository, Repositories.Public.StoredProcedureRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAddressRepository, Repositories.Dbo.AddressRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAspNetRoleRepository, Repositories.Dbo.AspNetRoleRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAspNetRoleClaimRepository, Repositories.Dbo.AspNetRoleClaimRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAspNetUserRepository, Repositories.Dbo.AspNetUserRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAspNetUserClaimRepository, Repositories.Dbo.AspNetUserClaimRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAspNetUserLoginRepository, Repositories.Dbo.AspNetUserLoginRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAspNetUserRoleRepository, Repositories.Dbo.AspNetUserRoleRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IAspNetUserTokenRepository, Repositories.Dbo.AspNetUserTokenRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IBrandRepository, Repositories.Dbo.BrandRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IChildRepository, Repositories.Dbo.ChildRepository>();
            services.AddTransient<Domain.Repositories.Dbo.ICustomerRepository, Repositories.Dbo.CustomerRepository>();
            services.AddTransient<IFKTableRepository, FKTableRepository>();
            services.AddTransient<Domain.Repositories.Dbo.ILegacyTableRepository, Repositories.Dbo.LegacyTableRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IOrderRepository, Repositories.Dbo.OrderRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IOrderItemRepository, Repositories.Dbo.OrderItemRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IParentRepository, Repositories.Dbo.ParentRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IPriceRepository, Repositories.Dbo.PriceRepository>();
            services.AddTransient<IPrimaryTableRepository, PrimaryTableRepository>();
            services.AddTransient<Domain.Repositories.Dbo.IProductRepository, Repositories.Dbo.ProductRepository>();
            services.AddTransient<ISelfReferenceTableRepository, SelfReferenceTableRepository>();
            services.AddTransient<Domain.Repositories.PsgSchema2.IBankRepository, Repositories.PsgSchema2.BankRepository>();
            services.AddTransient<IBank1Repository, Bank1Repository>();
            services.AddTransient<Domain.Repositories.PsgSchema2.ICustomerRepository, Repositories.PsgSchema2.CustomerRepository>();
            services.AddTransient<Domain.Repositories.PsgViews.IVwOrderRepository, Repositories.PsgViews.VwOrderRepository>();
            services.AddTransient<Domain.Repositories.Public.IAddressRepository, Repositories.Public.AddressRepository>();
            services.AddTransient<Domain.Repositories.Public.IAspNetRoleRepository, Repositories.Public.AspNetRoleRepository>();
            services.AddTransient<Domain.Repositories.Public.IAspNetRoleClaimRepository, Repositories.Public.AspNetRoleClaimRepository>();
            services.AddTransient<Domain.Repositories.Public.IAspNetUserRepository, Repositories.Public.AspNetUserRepository>();
            services.AddTransient<Domain.Repositories.Public.IAspNetUserClaimRepository, Repositories.Public.AspNetUserClaimRepository>();
            services.AddTransient<Domain.Repositories.Public.IAspNetUserLoginRepository, Repositories.Public.AspNetUserLoginRepository>();
            services.AddTransient<Domain.Repositories.Public.IAspNetUserRoleRepository, Repositories.Public.AspNetUserRoleRepository>();
            services.AddTransient<Domain.Repositories.Public.IAspNetUserTokenRepository, Repositories.Public.AspNetUserTokenRepository>();
            services.AddTransient<Domain.Repositories.Public.IBrandRepository, Repositories.Public.BrandRepository>();
            services.AddTransient<Domain.Repositories.Public.IChildRepository, Repositories.Public.ChildRepository>();
            services.AddTransient<Domain.Repositories.Public.ICustomerRepository, Repositories.Public.CustomerRepository>();
            services.AddTransient<IFktableRepository, FktableRepository>();
            services.AddTransient<Domain.Repositories.Public.ILegacyTableRepository, Repositories.Public.LegacyTableRepository>();
            services.AddTransient<Domain.Repositories.Public.IOrderRepository, Repositories.Public.OrderRepository>();
            services.AddTransient<Domain.Repositories.Public.IOrderItemRepository, Repositories.Public.OrderItemRepository>();
            services.AddTransient<Domain.Repositories.Public.IParentRepository, Repositories.Public.ParentRepository>();
            services.AddTransient<Domain.Repositories.Public.IPriceRepository, Repositories.Public.PriceRepository>();
            services.AddTransient<IPrimarytableRepository, PrimarytableRepository>();
            services.AddTransient<Domain.Repositories.Public.IProductRepository, Repositories.Public.ProductRepository>();
            services.AddTransient<ISelfreferencetableRepository, SelfreferencetableRepository>();
            services.AddTransient<Domain.Repositories.Schema2.IBankRepository, Repositories.Schema2.BankRepository>();
            services.AddTransient<IBanksRepository, BanksRepository>();
            services.AddTransient<Domain.Repositories.Schema2.ICustomerRepository, Repositories.Schema2.CustomerRepository>();
            services.AddTransient<ICustomersRepository, CustomersRepository>();
            services.AddTransient<Domain.Repositories.Views.IVwOrderRepository, Repositories.Views.VwOrderRepository>();
            services.AddScoped<IDomainEventService, DomainEventService>();
            return services;
        }
    }
}