using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class PrimaryTableConfiguration : IEntityTypeConfiguration<PrimaryTable>
    {
        public void Configure(EntityTypeBuilder<PrimaryTable> builder)
        {
            builder.ToTable("PrimaryTable", "dbo");

            builder.HasKey(x => x.PrimaryTableId);

            builder.Property(x => x.PrimaryTableId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(100)");

            builder.Property(x => x.FKTableId1)
                .IsRequired();

            builder.Property(x => x.FKWithTableId2)
                .IsRequired();

            builder.Property(x => x.FKTryTableId4)
                .IsRequired();

            builder.Property(x => x.FKThisTableId5)
                .IsRequired();

            builder.Property(x => x.FKAsTableId3)
                .IsRequired();

            builder.HasOne(x => x.FKWithTableId2FKTable)
                .WithMany()
                .HasForeignKey(x => x.FKWithTableId2)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FKTryTableId4FKTable)
                .WithMany()
                .HasForeignKey(x => x.FKTryTableId4)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FKThisTableId5FKTable)
                .WithMany()
                .HasForeignKey(x => x.FKThisTableId5)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FKTableId1FKTable)
                .WithMany()
                .HasForeignKey(x => x.FKTableId1)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FKAsTableId3FKTable)
                .WithMany()
                .HasForeignKey(x => x.FKAsTableId3)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}