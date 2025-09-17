using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents
{
    public interface IinvoiceDocument
    {
        string Id { get; }
        Guid CustomerId { get; }
        decimal TotalAmount { get; }
        IReadOnlyList<IItemDocument> Items { get; }
    }
}