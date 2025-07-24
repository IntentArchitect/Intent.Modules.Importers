using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.OrderId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(x => x.Quantity)
                .IsRequired()
                .HasColumnType("int4");

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("numeric");

            builder.Property(x => x.ProductId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.HasIndex(x => x.ProductId)
                .HasDatabaseName("IX_OrderItems_ProductId");

            builder.HasIndex(x => x.OrderId)
                .HasDatabaseName("IX_OrderItems_OrderId");

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Order)
                .WithMany()
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}