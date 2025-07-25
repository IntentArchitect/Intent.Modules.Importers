using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using RdbmsImporterTests.Application.Common.Interfaces;
using RdbmsImporterTests.Domain.Common;
using RdbmsImporterTests.Domain.Common.Interfaces;
using RdbmsImporterTests.Domain.Contracts.Dbo;
using RdbmsImporterTests.Domain.Contracts.Public;
using RdbmsImporterTests.Domain.Entities.Dbo;
using RdbmsImporterTests.Domain.Entities.Schema2;
using RdbmsImporterTests.Domain.Entities.Views;
using RdbmsImporterTests.Infrastructure.Persistence.Configurations.Dbo;
using RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public;
using RdbmsImporterTests.Infrastructure.Persistence.Configurations.Schema2;
using RdbmsImporterTests.Infrastructure.Persistence.Configurations.Views;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<Domain.Contracts.Dbo.GetCustomerOrdersResponse> DomainContractsDboGetCustomerOrdersResponses { get; set; }

        public DbSet<Domain.Contracts.Dbo.GetOrderItemDetailsResponse> DomainContractsDboGetOrderItemDetailsResponses { get; set; }
        public DbSet<UuidGenerateV1Response> UuidGenerateV1Responses { get; set; }
        public DbSet<UuidGenerateV1mcResponse> UuidGenerateV1mcResponses { get; set; }
        public DbSet<UuidGenerateV3Response> UuidGenerateV3Responses { get; set; }
        public DbSet<UuidGenerateV4Response> UuidGenerateV4Responses { get; set; }
        public DbSet<UuidGenerateV5Response> UuidGenerateV5Responses { get; set; }
        public DbSet<UuidNilResponse> UuidNilResponses { get; set; }
        public DbSet<UuidNsDnsResponse> UuidNsDnsResponses { get; set; }
        public DbSet<UuidNsOidResponse> UuidNsOidResponses { get; set; }
        public DbSet<UuidNsUrlResponse> UuidNsUrlResponses { get; set; }
        public DbSet<UuidNsX500Response> UuidNsX500Responses { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Domain.Entities.Dbo.Customer> DboCustomers { get; set; }
        public DbSet<LegacyTable> LegacyTables { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<VwOrder> VwOrders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Banks> Banks { get; set; }
        public DbSet<Domain.Entities.Schema2.Customer> Schema2Customers { get; set; }

        public override async Task<int> SaveChangesAsync(
            bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            await DispatchEventsAsync(cancellationToken);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            DispatchEventsAsync().GetAwaiter().GetResult();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureModel(modelBuilder);
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.GetCustomerOrdersResponseConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.GetOrderItemDetailsResponseConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AddressConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AspNetRoleConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AspNetRoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AspNetUserConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AspNetUserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AspNetUserLoginConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AspNetUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.AspNetUserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.BrandConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.ChildConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.LegacyTableConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.OrderConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.ParentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.PriceConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Dbo.ProductConfiguration());
            modelBuilder.ApplyConfiguration(new BankConfiguration());
            modelBuilder.ApplyConfiguration(new BanksConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Schema2.CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new CustomersConfiguration());
            modelBuilder.ApplyConfiguration(new VwOrderConfiguration());
        }

        [IntentManaged(Mode.Ignore)]
        private void ConfigureModel(ModelBuilder modelBuilder)
        {
            // Seed data
            // https://rehansaeed.com/migrating-to-entity-framework-core-seed-data/
            /* Eg.
            
            modelBuilder.Entity<Car>().HasData(
                new Car() { CarId = 1, Make = "Ferrari", Model = "F40" },
                new Car() { CarId = 2, Make = "Ferrari", Model = "F50" },
                new Car() { CarId = 3, Make = "Lamborghini", Model = "Countach" });
            */
        }

        private async Task DispatchEventsAsync(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEventEntity = ChangeTracker
                    .Entries<IHasDomainEvent>()
                    .SelectMany(x => x.Entity.DomainEvents)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity is null)
                {
                    break;
                }

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
            }
        }
    }
}