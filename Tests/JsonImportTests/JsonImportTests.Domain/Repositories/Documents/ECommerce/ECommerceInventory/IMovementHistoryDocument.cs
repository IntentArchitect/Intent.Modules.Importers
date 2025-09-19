using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IMovementHistoryDocument
    {
        Guid Id { get; }
        string Type { get; }
        decimal Quantity { get; }
        string Reason { get; }
        Guid ReferenceId { get; }
        DateTime Date { get; }
        string User { get; }
        string Notes { get; }
        decimal UnitCost { get; }
    }
}