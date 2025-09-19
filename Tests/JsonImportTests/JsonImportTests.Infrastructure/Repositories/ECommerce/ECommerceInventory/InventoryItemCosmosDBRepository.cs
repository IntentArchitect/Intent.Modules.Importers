using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.ECommerce.ECommerceInventory;
using JsonImportTests.Infrastructure.Persistence;
using JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBRepository", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Repositories.ECommerce.ECommerceInventory
{
    internal class InventoryItemCosmosDBRepository : CosmosDBRepositoryBase<InventoryItem, InventoryItemDocument, IInventoryItemDocument>, IInventoryItemRepository
    {
        public InventoryItemCosmosDBRepository(CosmosDBUnitOfWork unitOfWork,
            IRepository<InventoryItemDocument> cosmosRepository,
            ICosmosContainerProvider<InventoryItemDocument> containerProvider,
            IOptionsMonitor<RepositoryOptions> optionsMonitor) : base(unitOfWork, cosmosRepository, "id", containerProvider, optionsMonitor)
        {
        }

        public async Task<InventoryItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default) => await FindByIdAsync(id: id.ToString(), cancellationToken: cancellationToken);

        public async Task<List<InventoryItem>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default) => await FindByIdsAsync(ids.Select(id => id.ToString()).ToArray(), cancellationToken);
    }
}