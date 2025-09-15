using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class FktableConfiguration : IEntityTypeConfiguration<Fktable>
    {
        public void Configure(EntityTypeBuilder<Fktable> builder)
        {
            builder.ToTable("fktable", "public");

            builder.HasKey(x => x.Fktableid);

            builder.Property(x => x.Fktableid)
                .HasColumnName("fktableid")
                .HasColumnType("int4")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("varchar(100)")
                .HasColumnName("name");

            builder.HasIndex(x => x.Fktableid)
                .IsUnique()
                .HasDatabaseName("fktable_pkey");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}