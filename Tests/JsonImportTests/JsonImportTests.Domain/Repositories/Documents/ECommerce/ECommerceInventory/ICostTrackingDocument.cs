using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface ICostTrackingDocument
    {
        decimal AverageCost { get; }
        decimal LastCost { get; }
        decimal StandardCost { get; }
        decimal TotalValue { get; }
        string CostMethod { get; }
    }
}