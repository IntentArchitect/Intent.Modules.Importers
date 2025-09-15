using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class SelfReferenceTableConfiguration : IEntityTypeConfiguration<SelfReferenceTable>
    {
        public void Configure(EntityTypeBuilder<SelfReferenceTable> builder)
        {
            builder.ToTable("SelfReferenceTable", "dbo");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("ID");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.Email)
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.ManagerId);

            builder.HasOne(x => x.ManagerSelfReferenceTable)
                .WithMany()
                .HasForeignKey(x => x.ManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}