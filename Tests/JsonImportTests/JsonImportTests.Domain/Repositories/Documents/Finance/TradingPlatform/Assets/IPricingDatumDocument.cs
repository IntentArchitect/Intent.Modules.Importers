using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IPricingDatumDocument
    {
        decimal CurrentPrice { get; }
        decimal PreviousClose { get; }
        decimal DayHigh { get; }
        decimal DayLow { get; }
        decimal WeekHigh52 { get; }
        decimal WeekLow52 { get; }
        decimal Volume { get; }
        decimal AverageVolume10Day { get; }
        decimal AverageVolume3Month { get; }
        decimal Beta { get; }
        IPricingDataVolatilityDocument Volatility { get; }
    }
}