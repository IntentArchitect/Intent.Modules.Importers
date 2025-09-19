using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IOrderDetailDocument
    {
        string OrderType { get; }
        string TimeInForce { get; }
        string OrderPlacedBy { get; }
        string OrderChannel { get; }
        DateTime OrderTime { get; }
        DateTime ExecutionTime { get; }
        string Venue { get; }
        IExecutionQualityDocument ExecutionQuality { get; }
    }
}