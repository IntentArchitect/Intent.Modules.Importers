using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class GetOrderItemDetailsResponseConfiguration : IEntityTypeConfiguration<GetOrderItemDetailsResponse>
    {
        public void Configure(EntityTypeBuilder<GetOrderItemDetailsResponse> builder)
        {
            builder.HasNoKey().ToView(null);
            builder.Property(x => x.Amount).HasPrecision(16, 4);
        }
    }
}