using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IPerformanceVolatilityDocument
    {
        decimal Beta { get; }
        decimal StandardDeviation { get; }
        decimal SharpeRatio { get; }
        decimal MaxDrawdown { get; }
    }
}