using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public async Task<List<GetCustomerOrdersResponse>> GetCustomerOrders(
            Guid? customerId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.DomainContractsDboGetCustomerOrdersResponses
                .FromSqlInterpolated($"EXECUTE GetCustomerOrders {customerId}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<GetOrderItemDetailsResponse>> GetOrderItemDetails(
            Guid? orderId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.DomainContractsDboGetOrderItemDetailsResponses
                .FromSqlInterpolated($"EXECUTE GetOrderItemDetails {orderId}")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task InsertBrand(IEnumerable<BrandTypeModel>? brand, CancellationToken cancellationToken = default)
        {
            var brandParameter = new SqlParameter
            {
                IsNullable = true,
                SqlDbType = SqlDbType.Structured,
                Value = brand.ToDataTable(),
                TypeName = "BrandTypeModel"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"EXECUTE InsertBrand {brandParameter}", cancellationToken);
        }
    }
}