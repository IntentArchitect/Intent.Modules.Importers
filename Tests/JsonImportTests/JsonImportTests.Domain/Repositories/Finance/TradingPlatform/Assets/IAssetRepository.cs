using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Finance.TradingPlatform.Assets
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAssetRepository : ICosmosDBRepository<Asset, IAssetDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Asset?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Asset>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}