using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.ECommerce.Catalog.Categories;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Repositories.ECommerce.Catalog.Categories
{
    internal class categoryCategoryCosmosDBRepository : CosmosDBRepositoryBase<categoryCategory, categoryCategoryDocument, IcategoryCategoryDocument>, ICategoryCategoryRepository
    {
        public categoryCategoryCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<categoryCategoryDocument> cosmosRepository,
            ICosmosContainerProvider<categoryCategoryDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<categoryCategory?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<categoryCategory>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}