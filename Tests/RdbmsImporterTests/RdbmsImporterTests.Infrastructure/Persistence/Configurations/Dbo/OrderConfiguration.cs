using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CustomerId)
                .IsRequired();

            builder.Property(x => x.OrderDate)
                .IsRequired();

            builder.Property(x => x.RefNo)
                .IsRequired()
                .HasColumnType("nvarchar(450)");
            
            // Getting an ambiguous match error when using the IncludeProperties extension method
            // because SQL Server and PostgreSQL is installed.
            // We can look at fixing this once someone really needs it.
            // IntentIgnore(Match="builder.HasIndex(x => x.CustomerId)")
            SqlServerIndexBuilderExtensions
                .IncludeProperties(builder.HasIndex(x => x.CustomerId)
                    .HasDatabaseName("IX_Orders_CustomerId"), x => new { x.OrderDate });

            builder.HasIndex(x => x.RefNo)
                .IsUnique()
                .HasDatabaseName("IX_Orders_RefNo");

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}