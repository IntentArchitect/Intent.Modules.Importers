using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.EntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class AspNetUserConfiguration : IEntityTypeConfiguration<AspNetUser>
    {
        public void Configure(EntityTypeBuilder<AspNetUser> builder)
        {
            builder.ToTable("AspNetUsers", "public");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefreshToken);

            builder.Property(x => x.RefreshTokenExpired)
                .HasColumnType("timestamp");

            builder.Property(x => x.UserName);

            builder.Property(x => x.NormalizedUserName);

            builder.Property(x => x.Email);

            builder.Property(x => x.NormalizedEmail);

            builder.Property(x => x.EmailConfirmed)
                .IsRequired()
                .HasColumnType("bool");

            builder.Property(x => x.PasswordHash);

            builder.Property(x => x.SecurityStamp);

            builder.Property(x => x.ConcurrencyStamp);

            builder.Property(x => x.PhoneNumber);

            builder.Property(x => x.PhoneNumberConfirmed)
                .IsRequired()
                .HasColumnType("bool");

            builder.Property(x => x.TwoFactorEnabled)
                .IsRequired()
                .HasColumnType("bool");

            builder.Property(x => x.LockoutEnd)
                .HasColumnType("timestamptz");

            builder.Property(x => x.LockoutEnabled)
                .IsRequired()
                .HasColumnType("bool");

            builder.Property(x => x.AccessFailedCount)
                .IsRequired()
                .HasColumnType("int4");

            builder.HasIndex(x => x.NormalizedUserName)
                .IsUnique()
                .HasDatabaseName("UserNameIndex");

            builder.HasIndex(x => x.NormalizedEmail)
                .HasDatabaseName("EmailIndex");

            builder.Ignore(e => e.DomainEvents);
        }
    }
}