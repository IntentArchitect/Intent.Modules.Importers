using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IPerformanceDocument
    {
        decimal TotalValue { get; }
        decimal CashValue { get; }
        decimal InvestedValue { get; }
        decimal TotalGainLoss { get; }
        decimal TotalGainLossPercentage { get; }
        decimal DayChange { get; }
        decimal DayChangePercentage { get; }
        decimal YearToDateReturn { get; }
        decimal OneYearReturn { get; }
        decimal ThreeYearReturn { get; }
        decimal FiveYearReturn { get; }
        decimal InceptionReturn { get; }
        IPerformanceVolatilityDocument Volatility { get; }
    }
}