using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents
{
    public interface ICustomerDocument
    {
        string Id { get; }
        string Name { get; }
        string Email { get; }
        IReadOnlyList<IOrderDocument> Orders { get; }
    }
}