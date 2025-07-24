using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class LegacyTableConfiguration : IEntityTypeConfiguration<LegacyTable>
    {
        public void Configure(EntityTypeBuilder<LegacyTable> builder)
        {
            builder.ToTable("Legacy_Table", "public");

            builder.HasNoKey();

            builder.Property(x => x.LegacyId)
                .IsRequired()
                .HasColumnType("int4")
                .HasColumnName("LegacyID");

            builder.Property(x => x.LegacyColumn)
                .IsRequired();

            builder.Property(x => x.BadDate)
                .IsRequired()
                .HasColumnType("timestamp");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}