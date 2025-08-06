using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.CustomerId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(x => x.OrderDate)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Property(x => x.RefNo)
                .IsRequired()
                .HasColumnType("varchar(450)");

            builder.HasIndex(x => x.RefNo)
                .IsUnique()
                .HasDatabaseName("IX_Orders_RefNo");

            builder.HasIndex(x => new { x.CustomerId, x.OrderDate })
                .HasDatabaseName("IX_Orders_CustomerId");

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}