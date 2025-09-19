using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IStockLevelDocument
    {
        decimal Available { get; }
        decimal Reserved { get; }
        decimal OnOrder { get; }
        decimal Damaged { get; }
        decimal Total { get; }
        decimal MinimumLevel { get; }
        decimal MaximumLevel { get; }
        decimal ReorderPoint { get; }
        decimal ReorderQuantity { get; }
    }
}