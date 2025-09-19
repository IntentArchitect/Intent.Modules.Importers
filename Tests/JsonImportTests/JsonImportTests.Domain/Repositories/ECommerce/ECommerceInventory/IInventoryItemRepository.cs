using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.ECommerce.ECommerceInventory
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IInventoryItemRepository : ICosmosDBRepository<InventoryItem, IInventoryItemDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<InventoryItem?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<InventoryItem>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}