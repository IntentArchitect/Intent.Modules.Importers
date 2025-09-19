using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IHoldingDocument
    {
        Guid Id { get; }
        Guid AssetId { get; }
        string Symbol { get; }
        string AssetType { get; }
        decimal Quantity { get; }
        decimal AveragePrice { get; }
        decimal CurrentPrice { get; }
        decimal MarketValue { get; }
        decimal UnrealizedGainLoss { get; }
        decimal PercentageGainLoss { get; }
        decimal PercentOfPortfolio { get; }
        DateTime FirstPurchaseDate { get; }
        DateTime LastTransactionDate { get; }
        decimal DividendYield { get; }
        decimal AccruedDividends { get; }
    }
}