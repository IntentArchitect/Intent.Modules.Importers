using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Public
{
    public class UuidGenerateV3ResponseConfiguration : IEntityTypeConfiguration<UuidGenerateV3Response>
    {
        public void Configure(EntityTypeBuilder<UuidGenerateV3Response> builder)
        {
            builder.HasNoKey().ToView(null);
        }
    }
}