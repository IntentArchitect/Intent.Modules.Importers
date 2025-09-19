using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents
{
    public interface IItemDocument
    {
        string Id { get; }
        Guid ProductId { get; }
        decimal Quantity { get; }
        decimal Price { get; }
    }
}