using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios
{
    public interface IPortfolioRiskMetricDocument
    {
        decimal VaR95 { get; }
        decimal ConditionalVaR { get; }
        string RiskRating { get; }
        string ConcentrationRisk { get; }
        string LiquidityRisk { get; }
    }
}