using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IShippingDimensionDocument
    {
        decimal Length { get; }
        decimal Width { get; }
        decimal Height { get; }
        string Unit { get; }
    }
}