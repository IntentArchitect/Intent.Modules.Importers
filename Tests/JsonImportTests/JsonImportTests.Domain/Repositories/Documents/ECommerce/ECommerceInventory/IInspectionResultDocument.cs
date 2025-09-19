using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory
{
    public interface IInspectionResultDocument
    {
        string Grade { get; }
        decimal DefectCount { get; }
        string Notes { get; }
        string InspectedBy { get; }
    }
}