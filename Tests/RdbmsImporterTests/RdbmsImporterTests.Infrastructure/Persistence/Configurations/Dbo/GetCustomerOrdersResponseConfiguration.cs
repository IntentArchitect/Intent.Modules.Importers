using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.DataContractEntityTypeConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Persistence.Configurations.Dbo
{
    public class GetCustomerOrdersResponseConfiguration : IEntityTypeConfiguration<GetCustomerOrdersResponse>
    {
        public void Configure(EntityTypeBuilder<GetCustomerOrdersResponse> builder)
        {
            builder.HasNoKey().ToView(null);
        }
    }
}