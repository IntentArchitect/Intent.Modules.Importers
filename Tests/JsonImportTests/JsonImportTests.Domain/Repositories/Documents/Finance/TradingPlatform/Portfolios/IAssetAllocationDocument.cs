using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IAssetAllocationDocument
    {
        IStockDocument Stocks { get; }
        IOtherDocument Other { get; }
        ICashDocument Cash { get; }
        IBondDocument Bonds { get; }
    }
}