using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IRatingDistributionDocument
    {
        decimal Star5 { get; }
        decimal Star4 { get; }
        decimal Star3 { get; }
        decimal Star2 { get; }
        decimal Star1 { get; }
    }
}