using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using RdbmsImporterTests.Domain.Contracts.Public;
using RdbmsImporterTests.Domain.Repositories.Public;
using RdbmsImporterTests.Infrastructure.Persistence;
using RdbmsImporterTests.Infrastructure.Repositories.ExtensionMethods.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.Public
{
    public class StoredProcedureRepository : IStoredProcedureRepository
    {
        private readonly PostgresAppDbContext _dbContext;

        public StoredProcedureRepository(PostgresAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<GetCustomerOrdersResponse>> GetCustomerOrders(
            Guid? customer_id,
            Guid? customerId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.GetCustomerOrdersResponses
                .FromSqlInterpolated($"SELECT * FROM public.GetCustomerOrders({customer_id}, {customerId})")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<GetOrderItemDetailsResponse>> GetOrderItemDetails(
            Guid? order_id,
            Guid? orderId,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.GetOrderItemDetailsResponses
                .FromSqlInterpolated($"SELECT * FROM public.GetOrderItemDetails({order_id}, {orderId})")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task InsertBrand(IEnumerable<BrandTypeModel>? brands, CancellationToken cancellationToken = default)
        {
            var brandsParameter = new NpgsqlParameter
            {
                IsNullable = true,
                NpgsqlDbType = NpgsqlDbType.Unknown,
                Value = brands.ToDataTable(),
                DataTypeName = "BrandType"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"CALL public.InsertBrand({brandsParameter})", cancellationToken);
        }

        public async Task InsertBrandFromTemp(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"CALL public.InsertBrandFromTemp()", cancellationToken);
        }

        public async Task<List<UuidGenerateV1Response>> UuidGenerateV1(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV1Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v1()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidGenerateV1mcResponse>> UuidGenerateV1mc(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV1mcResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v1mc()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidGenerateV3Response>> UuidGenerateV3(
            Guid? @namespace,
            string? name,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV3Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v3({@namespace}, {name})")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidGenerateV4Response>> UuidGenerateV4(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV4Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v4()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidGenerateV5Response>> UuidGenerateV5(
            Guid? @namespace,
            string? name,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV5Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v5({@namespace}, {name})")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidNilResponse>> UuidNil(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNilResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_nil()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidNsDnsResponse>> UuidNsDns(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsDnsResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_dns()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidNsOidResponse>> UuidNsOid(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsOidResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_oid()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidNsUrlResponse>> UuidNsUrl(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsUrlResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_url()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }

        public async Task<List<UuidNsX500Response>> UuidNsX500(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsX500Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_x500()")
                .IgnoreQueryFilters()
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}