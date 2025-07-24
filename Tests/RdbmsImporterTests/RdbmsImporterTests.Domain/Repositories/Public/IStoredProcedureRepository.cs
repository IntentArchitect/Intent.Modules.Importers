using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Contracts.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Repositories.Public
{
    public interface IStoredProcedureRepository
    {
        Task<IReadOnlyCollection<GetCustomerOrdersResponse>> GetCustomerOrders(Guid? customer_id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<GetOrderItemDetailsResponse>> GetOrderItemDetails(Guid? order_id, CancellationToken cancellationToken = default);
        Task InsertBrand(IEnumerable<BrandTypeModel>? brands, CancellationToken cancellationToken = default);
        Task InsertBrandFromTemp(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidGenerateV1Response>> UuidGenerateV1(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidGenerateV1mcResponse>> UuidGenerateV1mc(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidGenerateV3Response>> UuidGenerateV3(Guid? @namespace, string? name, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidGenerateV4Response>> UuidGenerateV4(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidGenerateV5Response>> UuidGenerateV5(Guid? @namespace, string? name, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidNilResponse>> UuidNil(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidNsDnsResponse>> UuidNsDns(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidNsOidResponse>> UuidNsOid(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidNsUrlResponse>> UuidNsUrl(CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<UuidNsX500Response>> UuidNsX500(CancellationToken cancellationToken = default);
    }
}