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
    public interface IStoredProcedureRepository
    {
        List<GetCustomerOrdersResponse> GetCustomerOrders(Guid? customerId);
        Task<IReadOnlyCollection<GetCustomerOrdersResponse>> GetCustomerOrders(Guid customerId, CancellationToken cancellationToken = default);
        List<GetOrderItemDetailsResponse> GetOrderItemDetails(Guid? orderId);
        Task<IReadOnlyCollection<GetOrderItemDetailsResponse>> GetOrderItemDetails(Guid orderId, CancellationToken cancellationToken = default);
        void InsertBrand(IEnumerable<BrandTypeModel>? brand);
        Task InsertBrand(IEnumerable<BrandType> brand, CancellationToken cancellationToken = default);
    }
}