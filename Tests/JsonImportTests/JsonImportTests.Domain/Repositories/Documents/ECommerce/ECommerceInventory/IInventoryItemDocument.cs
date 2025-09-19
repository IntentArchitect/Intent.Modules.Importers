using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IInventoryItemDocument
    {
        string Id { get; }
        Guid ProductId { get; }
        Guid VariantId { get; }
        string SKU { get; }
        DateTime LastUpdated { get; }
        string UpdatedBy { get; }
        decimal Version { get; }
        IStockLevelDocument StockLevels { get; }
        IReadOnlyList<IReservationDocument> Reservations { get; }
        IQualityControlDocument QualityControl { get; }
        IReadOnlyList<IMovementHistoryDocument> MovementHistory { get; }
        IInventoryItemLocationDocument Location { get; }
        ICostTrackingDocument CostTracking { get; }
        IReadOnlyList<IAlertDocument> Alerts { get; }
    }
}