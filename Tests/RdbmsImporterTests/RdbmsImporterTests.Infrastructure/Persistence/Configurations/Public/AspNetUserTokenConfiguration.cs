using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class AspNetUserTokenConfiguration : IEntityTypeConfiguration<AspNetUserToken>
    {
        public void Configure(EntityTypeBuilder<AspNetUserToken> builder)
        {
            builder.ToTable("AspNetUserTokens", "public");

            builder.HasKey(x => new { x.UserId, x.LoginProvider, x.Name });

            builder.Property(x => x.Value)
                .HasColumnType("text");

            builder.HasIndex(x => new { x.UserId, x.LoginProvider, x.Name })
                .IsUnique()
                .HasDatabaseName("PK_AspNetUserTokens");

            builder.HasOne(x => x.UserAspNetUser)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}