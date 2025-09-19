using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface IFeaturedProductDocument
    {
        string Id { get; }
        Guid ProductId { get; }
        decimal Position { get; }
        bool IsSponsored { get; }
    }
}