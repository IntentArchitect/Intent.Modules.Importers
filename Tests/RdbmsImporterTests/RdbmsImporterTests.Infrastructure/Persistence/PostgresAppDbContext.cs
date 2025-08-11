using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using RdbmsImporterTests.Application.Common.Interfaces;
using RdbmsImporterTests.Domain.Common;
using RdbmsImporterTests.Domain.Common.Interfaces;
using RdbmsImporterTests.Domain.Contracts.Public;
using RdbmsImporterTests.Domain.Entities.PsgSchema2;
using RdbmsImporterTests.Domain.Entities.PsgViews;
using RdbmsImporterTests.Domain.Entities.Public;
using RdbmsImporterTests.Infrastructure.Persistence.Configurations.PsgSchema2;
using RdbmsImporterTests.Infrastructure.Persistence.Configurations.PsgViews;
using RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.DbContext", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence
{
    public class PostgresAppDbContext : DbContext, IUnitOfWork
    {
        private readonly IDomainEventService _domainEventService;

        public PostgresAppDbContext(DbContextOptions<PostgresAppDbContext> options, IDomainEventService domainEventService) : base(options)
        {
            _domainEventService = domainEventService;
        }

        public DbSet<GetCustomerOrdersResponse> GetCustomerOrdersResponses { get; set; }
        public DbSet<GetOrderItemDetailsResponse> GetOrderItemDetailsResponses { get; set; }
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
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Bank1> Bank1s { get; set; }
        public DbSet<Domain.Entities.PsgSchema2.Customer> PsgSchema2Customers { get; set; }
        public DbSet<VwOrder> VwOrders { get; set; }
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
        public DbSet<Domain.Entities.Public.Customer> PublicCustomers { get; set; }
        public DbSet<LegacyTable> LegacyTables { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Product> Products { get; set; }

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
            modelBuilder.ApplyConfiguration(new GetCustomerOrdersResponseConfiguration());
            modelBuilder.ApplyConfiguration(new GetOrderItemDetailsResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidGenerateV1ResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidGenerateV1mcResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidGenerateV3ResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidGenerateV4ResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidGenerateV5ResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidNilResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidNsDnsResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidNsOidResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidNsUrlResponseConfiguration());
            modelBuilder.ApplyConfiguration(new UuidNsX500ResponseConfiguration());
            modelBuilder.ApplyConfiguration(new BankConfiguration());
            modelBuilder.ApplyConfiguration(new Bank1Configuration());
            modelBuilder.ApplyConfiguration(new Configurations.PsgSchema2.CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new VwOrderConfiguration());
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetRoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserLoginConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new ChildConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.Public.CustomerConfiguration());
            modelBuilder.ApplyConfiguration(new LegacyTableConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new ParentConfiguration());
            modelBuilder.ApplyConfiguration(new PriceConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
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