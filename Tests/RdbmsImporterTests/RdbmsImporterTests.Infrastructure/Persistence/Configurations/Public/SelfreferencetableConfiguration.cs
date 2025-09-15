using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class SelfreferencetableConfiguration : IEntityTypeConfiguration<Selfreferencetable>
    {
        public void Configure(EntityTypeBuilder<Selfreferencetable> builder)
        {
            builder.ToTable("selfreferencetable", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("uuid");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("varchar(50)")
                .HasColumnName("name");

            builder.Property(x => x.Email)
                .HasColumnType("varchar(50)")
                .HasColumnName("email");

            builder.Property(x => x.Managerid)
                .HasColumnType("uuid")
                .HasColumnName("managerid");

            builder.HasIndex(x => x.Id)
                .IsUnique()
                .HasDatabaseName("pk_selfreferencetable");

            builder.HasOne(x => x.Managerselfreferencetable)
                .WithMany()
                .HasForeignKey(x => x.Managerid)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}