using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IVariantsDimensionDocument
    {
        decimal Length { get; }
        decimal Width { get; }
        decimal Height { get; }
        decimal Weight { get; }
        string Unit { get; }
    }
}