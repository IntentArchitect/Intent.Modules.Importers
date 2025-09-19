using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IQuantityDocument
    {
        decimal OrderedQuantity { get; }
        decimal ExecutedQuantity { get; }
        decimal RemainingQuantity { get; }
        decimal CancelledQuantity { get; }
    }
}