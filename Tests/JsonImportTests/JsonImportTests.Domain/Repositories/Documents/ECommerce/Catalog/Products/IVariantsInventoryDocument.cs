using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IVariantsInventoryDocument
    {
        decimal Quantity { get; }
        decimal ReservedQuantity { get; }
        decimal MinStockLevel { get; }
        decimal MaxStockLevel { get; }
        decimal ReorderPoint { get; }
        decimal ReorderQuantity { get; }
    }
}