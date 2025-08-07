using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Dbo;
using RdbmsImporterTests.Domain.Repositories.Dbo;
using RdbmsImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.Dbo
{
    public class OperationRepository : IOperationRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OperationRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<GetCustomerOrdersResponse> GetCustomerOrders(Guid customerId)
        {
            // TODO: Implement GetCustomerOrders (OperationRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<GetOrderItemDetailsResponse> GetOrderItemDetails(Guid orderId)
        {
            // TODO: Implement GetOrderItemDetails (OperationRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void InsertBrand(IEnumerable<BrandTypeModel> brand)
        {
            // TODO: Implement InsertBrand (OperationRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}