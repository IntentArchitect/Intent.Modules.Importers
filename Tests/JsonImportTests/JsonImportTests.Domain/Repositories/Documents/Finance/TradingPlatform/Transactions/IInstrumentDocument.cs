using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IInstrumentDocument
    {
        string Symbol { get; }
        string Name { get; }
        string AssetType { get; }
        string ISIN { get; }
        string Exchange { get; }
        string Currency { get; }
    }
}