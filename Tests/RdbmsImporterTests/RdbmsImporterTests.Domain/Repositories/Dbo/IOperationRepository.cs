using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Repositories.Dbo
{
    public interface IOperationRepository
    {
        List<GetCustomerOrdersResponse> GetCustomerOrders(Guid customerId);
        List<GetOrderItemDetailsResponse> GetOrderItemDetails(Guid orderId);
        void InsertBrand(IEnumerable<BrandType> brand);
    }
}