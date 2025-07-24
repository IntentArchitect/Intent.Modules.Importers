using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.BrandId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Property(x => x.Description)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasColumnType("bool");

            builder.HasIndex(x => x.BrandId)
                .HasDatabaseName("IX_Products_BrandId");

            builder.HasOne(x => x.Brand)
                .WithMany()
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}