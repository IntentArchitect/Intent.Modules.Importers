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

        public async Task<IReadOnlyCollection<GetCustomerOrdersResponse>> GetCustomerOrders(
            Guid? customer_id,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.Domain.Contracts.Public.GetCustomerOrdersResponses
                .FromSqlInterpolated($"SELECT * FROM GetCustomerOrders({customer_id})")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<GetOrderItemDetailsResponse>> GetOrderItemDetails(
            Guid? order_id,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.Domain.Contracts.Public.GetOrderItemDetailsResponses
                .FromSqlInterpolated($"SELECT * FROM GetOrderItemDetails({order_id})")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task InsertBrand(IEnumerable<BrandTypeModel>? brands, CancellationToken cancellationToken = default)
        {
            var brandsParameter = new NpgsqlParameter
            {
                IsNullable = true,
                NpgsqlDbType = NpgsqlDbType.Unknown,
                Value = brands.ToDataTable(),
                TypeName = "BrandTypeModel"
            };

            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"CALL InsertBrand({brandsParameter})", cancellationToken);
        }

        public async Task InsertBrandFromTemp(CancellationToken cancellationToken = default)
        {
            await _dbContext.Database.ExecuteSqlInterpolatedAsync($"CALL InsertBrandFromTemp()", cancellationToken);
        }

        public async Task<IReadOnlyCollection<UuidGenerateV1Response>> UuidGenerateV1(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV1Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v1()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidGenerateV1mcResponse>> UuidGenerateV1mc(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV1mcResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v1mc()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidGenerateV3Response>> UuidGenerateV3(
            Guid? @namespace,
            string? name,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV3Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v3({namespace}, {name})")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidGenerateV4Response>> UuidGenerateV4(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV4Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v4()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidGenerateV5Response>> UuidGenerateV5(
            Guid? @namespace,
            string? name,
            CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidGenerateV5Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_generate_v5({namespace}, {name})")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidNilResponse>> UuidNil(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNilResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_nil()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidNsDnsResponse>> UuidNsDns(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsDnsResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_dns()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidNsOidResponse>> UuidNsOid(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsOidResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_oid()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidNsUrlResponse>> UuidNsUrl(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsUrlResponses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_url()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }

        public async Task<IReadOnlyCollection<UuidNsX500Response>> UuidNsX500(CancellationToken cancellationToken = default)
        {
            var results = await _dbContext.UuidNsX500Responses
                .FromSqlInterpolated($"SELECT * FROM uuid_ns_x500()")
                .IgnoreQueryFilters()
                .ToArrayAsync(cancellationToken);

            return results;
        }
    }
}