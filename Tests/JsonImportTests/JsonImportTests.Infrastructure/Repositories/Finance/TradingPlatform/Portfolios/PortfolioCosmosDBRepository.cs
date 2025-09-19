using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Repositories.Finance.TradingPlatform.Portfolios
{
    internal class PortfolioCosmosDBRepository : CosmosDBRepositoryBase<Portfolio, PortfolioDocument, IPortfolioDocument>, IPortfolioRepository
    {
        public PortfolioCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<PortfolioDocument> cosmosRepository,
            ICosmosContainerProvider<PortfolioDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<Portfolio?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<Portfolio>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}