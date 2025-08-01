using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class AspNetRoleClaimConfiguration : IEntityTypeConfiguration<AspNetRoleClaim>
    {
        public void Configure(EntityTypeBuilder<AspNetRoleClaim> builder)
        {
            builder.ToTable("AspNetRoleClaims", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("int4")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.RoleId)
                .IsRequired();

            builder.Property(x => x.ClaimType);

            builder.Property(x => x.ClaimValue);

            builder.HasIndex(x => x.RoleId)
                .HasDatabaseName("IX_AspNetRoleClaims_RoleId");

            builder.HasOne(x => x.RoleIdAspNetRole)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}