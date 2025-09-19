using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IFundamentalDocument
    {
        decimal MarketCapitalization { get; }
        decimal EnterpriseValue { get; }
        decimal PERatio { get; }
        decimal EarningsPerShare { get; }
        decimal BookValuePerShare { get; }
        decimal PriceToBook { get; }
        decimal PriceToSales { get; }
        decimal DividendPerShare { get; }
        decimal DividendYield { get; }
        decimal PayoutRatio { get; }
        decimal ReturnOnEquity { get; }
        decimal ReturnOnAssets { get; }
        decimal DebtToEquity { get; }
        decimal CurrentRatio { get; }
        decimal QuickRatio { get; }
    }
}