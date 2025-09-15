using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class FKTableConfiguration : IEntityTypeConfiguration<FKTable>
    {
        public void Configure(EntityTypeBuilder<FKTable> builder)
        {
            builder.ToTable("FKTable", "dbo");

            builder.HasKey(x => x.FKTableId);

            builder.Property(x => x.FKTableId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(100)");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}