using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IRatingDistributionDocument
    {
        decimal 5Star { get; }
    decimal 4Star { get; }
decimal 3Star { get; }
        decimal 2Star { get; }
        decimal 1Star { get; }
    }
}