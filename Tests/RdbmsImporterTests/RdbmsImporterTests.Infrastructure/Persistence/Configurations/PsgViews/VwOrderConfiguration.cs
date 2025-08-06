using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.PsgViews;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.PsgViews
{
    public class VwOrderConfiguration : IEntityTypeConfiguration<VwOrder>
    {
        public void Configure(EntityTypeBuilder<VwOrder> builder)
        {
            builder.ToView("vwOrders", "views");

            builder.HasNoKey();

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.CustomerId)
                .HasColumnType("uuid");

            builder.Property(x => x.OrderDate)
                .HasColumnType("timestamp");

            builder.Property(x => x.RefNo)
                .HasColumnType("varchar(450)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}