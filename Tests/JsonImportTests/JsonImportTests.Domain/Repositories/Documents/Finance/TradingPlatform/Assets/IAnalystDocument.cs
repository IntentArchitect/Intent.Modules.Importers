using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IAnalystDocument
    {
        string Consensus { get; }
        decimal TargetPrice { get; }
        decimal PriceTargetHigh { get; }
        decimal PriceTargetLow { get; }
        IRecommendationDocument Recommendations { get; }
    }
}