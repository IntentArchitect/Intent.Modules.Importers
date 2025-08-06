using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(x => x.Surname)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.Email)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasColumnType("bool");

            builder.Property(x => x.PreferencesSpecials)
                .HasColumnType("bool")
                .HasColumnName("Preferences_Specials");

            builder.Property(x => x.PreferencesNewsletter)
                .HasColumnType("bool")
                .HasColumnName("Preferences_Newsletter");

            builder.HasIndex(x => x.Id)
                .IsUnique()
                .HasDatabaseName("PK_Customers");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}