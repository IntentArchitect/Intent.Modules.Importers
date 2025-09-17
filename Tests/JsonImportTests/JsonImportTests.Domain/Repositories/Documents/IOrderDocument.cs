using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents
{
    public interface IOrderDocument
    {
        Guid Id { get; }
        decimal Amount { get; }
    }
}