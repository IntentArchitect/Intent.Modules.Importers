using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TestDataGenerator.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace TestDataGenerator.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefNo)
                .IsRequired();

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.OwnsMany(x => x.OrderLines, ConfigureOrderLines);

            builder.Ignore(e => e.DomainEvents);
        }

        public static void ConfigureOrderLines(OwnedNavigationBuilder<Order, OrderLine> builder)
        {
            builder.WithOwner()
                .HasForeignKey(x => x.OrderId);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.OrderId)
                .IsRequired();

            builder.Property(x => x.ProductId)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}