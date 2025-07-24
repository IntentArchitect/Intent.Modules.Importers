using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class PriceConfiguration : IEntityTypeConfiguration<Price>
    {
        public void Configure(EntityTypeBuilder<Price> builder)
        {
            builder.ToTable("Prices", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.ProductId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(x => x.ActiveFrom)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("numeric");

            builder.HasIndex(x => x.ProductId)
                .HasDatabaseName("IX_Prices_ProductId");

            builder.HasOne(x => x.Product)
                .WithMany()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}