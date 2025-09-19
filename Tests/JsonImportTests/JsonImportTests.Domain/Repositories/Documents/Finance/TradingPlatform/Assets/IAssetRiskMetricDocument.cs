using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IAssetRiskMetricDocument
    {
        decimal ESGScore { get; }
        string LiquidityRisk { get; }
        string ConcentrationRisk { get; }
        string RegulatoryRisk { get; }
        ICreditRatingDocument CreditRating { get; }
    }
}