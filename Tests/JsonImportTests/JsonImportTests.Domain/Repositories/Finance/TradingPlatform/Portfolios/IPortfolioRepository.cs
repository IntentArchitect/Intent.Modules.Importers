using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Finance.TradingPlatform.Portfolios
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPortfolioRepository : ICosmosDBRepository<Portfolio, IPortfolioDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Portfolio?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Portfolio>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}