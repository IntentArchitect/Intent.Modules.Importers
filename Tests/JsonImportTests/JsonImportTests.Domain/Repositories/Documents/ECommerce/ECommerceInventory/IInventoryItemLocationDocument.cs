using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IInventoryItemLocationDocument
    {
        Guid WarehouseId { get; }
        string WarehouseName { get; }
        string Zone { get; }
        string Aisle { get; }
        string Shelf { get; }
        string Bin { get; }
    }
}