using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IQualityControlDocument
    {
        DateTime LastInspectionDate { get; }
        DateTime ExpirationDate { get; }
        string BatchNumber { get; }
        IReadOnlyList<string> SerialNumbers { get; }
        IInspectionResultDocument InspectionResults { get; }
    }
}