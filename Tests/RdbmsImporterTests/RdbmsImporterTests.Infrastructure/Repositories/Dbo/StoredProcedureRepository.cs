using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RdbmsImporterTests.Domain.Contracts.Dbo;
using RdbmsImporterTests.Domain.Repositories.Dbo;
using RdbmsImporterTests.Infrastructure.Persistence;
using RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.Dbo
{
    public class StoredProcedureRepository : IStoredProcedureRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public StoredProcedureRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IReadOnlyCollection<GetCustomerOrdersResponse>> GetCustomerOrders(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.DomainContractsDboGetCustomerOrdersResponses
                .FromSqlInterpolated($"EXECUTE GetCustomerOrders {customerId}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<GetCustomerOrdersResponse> GetCustomerOrders(Guid? customerId)
        {
            // TODO: Implement GetCustomerOrders (StoredProcedureRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        public async Task<IReadOnlyCollection<GetOrderItemDetailsResponse>> GetOrderItemDetails(
            Guid orderId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.DomainContractsDboGetOrderItemDetailsResponses
                .FromSqlInterpolated($"EXECUTE GetOrderItemDetails {orderId}")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<GetOrderItemDetailsResponse> GetOrderItemDetails(Guid? orderId)
        {
            // TODO: Implement GetOrderItemDetails (StoredProcedureRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        public async Task InsertBrand(IEnumerable<BrandType> brand, CancellationToken cancellationToken = default)
        {
            var brandParameter = new SqlParameter
            {
                IsNullable = false,
                SqlDbType = SqlDbType.Structured,
                Value = brand.ToDataTable(),
                TypeName = "BrandType"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE InsertBrand {brandParameter}", cancellationToken);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public void InsertBrand(IEnumerable<BrandTypeModel>? brand)
        {
            // TODO: Implement InsertBrand (StoredProcedureRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}