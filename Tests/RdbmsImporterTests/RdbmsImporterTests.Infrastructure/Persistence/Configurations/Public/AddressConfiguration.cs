using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.CustomerId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(x => x.Line1)
                .IsRequired();

            builder.Property(x => x.Line2)
                .IsRequired();

            builder.Property(x => x.City)
                .IsRequired();

            builder.Property(x => x.PostalCode)
                .IsRequired();

            builder.Property(x => x.AddressType)
                .IsRequired()
                .HasColumnType("int4");

            builder.HasIndex(x => x.CustomerId)
                .HasDatabaseName("IX_Addresses_CustomerId");

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}