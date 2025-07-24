using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.PsgSchema2;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.PsgSchema2
{
    public class BankConfiguration : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.ToTable("Bank", "schema2");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.Name)
                .IsRequired();

            builder.Ignore(e => e.DomainEvents);
        }
    }
}