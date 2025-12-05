using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Repositories.Dbo
{
    public interface IElementRepository
    {
        Task<List<GetCustomerOrdersResponse>> GetCustomerOrders(Guid? customerId, CancellationToken cancellationToken = default);
        Task<List<GetOrderItemDetailsResponse>> GetOrderItemDetails(Guid? orderId, CancellationToken cancellationToken = default);
        Task InsertBrand(IEnumerable<BrandTypeModel>? brand, CancellationToken cancellationToken = default);
    }
}