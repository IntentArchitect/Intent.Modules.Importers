using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class AspNetRoleConfiguration : IEntityTypeConfiguration<AspNetRole>
    {
        public void Configure(EntityTypeBuilder<AspNetRole> builder)
        {
            builder.ToTable("AspNetRoles", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name);

            builder.Property(x => x.NormalizedName);

            builder.Property(x => x.ConcurrencyStamp);

            builder.HasIndex(x => x.NormalizedName)
                .IsUnique()
                .HasDatabaseName("RoleNameIndex");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}