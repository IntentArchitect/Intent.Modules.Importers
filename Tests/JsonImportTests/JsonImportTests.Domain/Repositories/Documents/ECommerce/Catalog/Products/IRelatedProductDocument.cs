using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IRelatedProductDocument
    {
        string Id { get; }
        Guid ProductId { get; }
        string RelationType { get; }
        decimal Position { get; }
    }
}