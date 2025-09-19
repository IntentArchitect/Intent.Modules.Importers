using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface ICounterpartyDocument
    {
        string BrokerId { get; }
        string BrokerName { get; }
        string DealerId { get; }
        string DealerName { get; }
        string PrimeBroker { get; }
    }
}