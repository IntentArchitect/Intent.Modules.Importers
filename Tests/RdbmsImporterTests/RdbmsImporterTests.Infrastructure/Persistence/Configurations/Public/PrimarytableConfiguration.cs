using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class PrimarytableConfiguration : IEntityTypeConfiguration<Primarytable>
    {
        public void Configure(EntityTypeBuilder<Primarytable> builder)
        {
            builder.ToTable("primarytable", "public");

            builder.HasKey(x => x.Primarytableid);

            builder.Property(x => x.Primarytableid)
                .HasColumnName("primarytableid")
                .HasColumnType("int4")
                .HasDefaultValueSql("nextval('primarytable_primarytableid_seq'::regclass)")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("varchar(100)")
                .HasColumnName("name");

            builder.Property(x => x.Fktableid1)
                .IsRequired()
                .HasColumnType("int4")
                .HasColumnName("fktableid1");

            builder.Property(x => x.Fkwithtableid2)
                .IsRequired()
                .HasColumnType("int4")
                .HasColumnName("fkwithtableid2");

            builder.Property(x => x.Fktrytableid4)
                .IsRequired()
                .HasColumnType("int4")
                .HasColumnName("fktrytableid4");

            builder.Property(x => x.Fkthistableid5)
                .IsRequired()
                .HasColumnType("int4")
                .HasColumnName("fkthistableid5");

            builder.Property(x => x.Fkastableid3)
                .IsRequired()
                .HasColumnType("int4")
                .HasColumnName("fkastableid3");

            builder.HasIndex(x => x.Primarytableid)
                .IsUnique()
                .HasDatabaseName("primarytable_pkey");

            builder.HasOne(x => x.Fkwithtableid2fktable)
                .WithMany()
                .HasForeignKey(x => x.Fkwithtableid2)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Fktrytableid4fktable)
                .WithMany()
                .HasForeignKey(x => x.Fktrytableid4)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Fkthistableid5fktable)
                .WithMany()
                .HasForeignKey(x => x.Fkthistableid5)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Fktableid1fktable)
                .WithMany()
                .HasForeignKey(x => x.Fktableid1)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Fkastableid3fktable)
                .WithMany()
                .HasForeignKey(x => x.Fkastableid3)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}