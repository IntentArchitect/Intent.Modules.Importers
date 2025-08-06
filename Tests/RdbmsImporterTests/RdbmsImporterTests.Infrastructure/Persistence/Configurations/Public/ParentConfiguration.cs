using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class ParentConfiguration : IEntityTypeConfiguration<Parent>
    {
        public void Configure(EntityTypeBuilder<Parent> builder)
        {
            builder.ToTable("Parents", "public");

            builder.HasKey(x => new { x.Id, x.Id2 });

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.Id2)
                .HasColumnType("uuid");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("text");

            builder.HasIndex(x => new { x.Id, x.Id2 })
                .IsUnique()
                .HasDatabaseName("PK_Parents");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}