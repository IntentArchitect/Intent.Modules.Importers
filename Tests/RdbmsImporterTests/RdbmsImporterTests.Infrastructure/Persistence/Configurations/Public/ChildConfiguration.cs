using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class ChildConfiguration : IEntityTypeConfiguration<Child>
    {
        public void Configure(EntityTypeBuilder<Child> builder)
        {
            builder.ToTable("Children", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid");

            builder.Property(x => x.ParentId)
                .IsRequired()
                .HasColumnType("uuid");

            builder.Property(x => x.ParentId2)
                .IsRequired()
                .HasColumnType("uuid");

            builder.HasIndex(x => new { x.ParentId, x.ParentId2 })
                .HasDatabaseName("IX_Children_ParentId_ParentId2");

            builder.HasOne(x => x.Parent)
                .WithMany()
                .HasForeignKey(x => new { x.ParentId, x.ParentId2 })
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}