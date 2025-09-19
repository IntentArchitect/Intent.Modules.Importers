using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IAlertDocument
    {
        string Id { get; }
        string Type { get; }
        string Message { get; }
        string Severity { get; }
        DateTime CreatedDate { get; }
        bool IsAcknowledged { get; }
    }
}