using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IReservationDocument
    {
        Guid Id { get; }
        Guid OrderId { get; }
        decimal Quantity { get; }
        DateTime ReservedDate { get; }
        DateTime ExpirationDate { get; }
        string Status { get; }
        string ReservedBy { get; }
    }
}