using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class AspNetUserLoginConfiguration : IEntityTypeConfiguration<AspNetUserLogin>
    {
        public void Configure(EntityTypeBuilder<AspNetUserLogin> builder)
        {
            builder.ToTable("AspNetUserLogins", "public");

            builder.HasKey(x => new { x.LoginProvider, x.ProviderKey });

            builder.Property(x => x.ProviderDisplayName)
                .HasColumnType("text");

            builder.Property(x => x.UserId)
                .IsRequired()
                .HasColumnType("varchar(450)");

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_AspNetUserLogins_UserId");

            builder.HasOne(x => x.UserAspNetUser)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Ignore(e => e.DomainEvents);
        }
    }
}