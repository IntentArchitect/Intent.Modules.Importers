using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class AspNetUserClaimConfiguration : IEntityTypeConfiguration<AspNetUserClaim>
    {
        public void Configure(EntityTypeBuilder<AspNetUserClaim> builder)
        {
            builder.ToTable("AspNetUserClaims", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("int4")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.ClaimType);

            builder.Property(x => x.ClaimValue);

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_AspNetUserClaims_UserId");

            builder.HasOne(x => x.UserAspNetUser)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}