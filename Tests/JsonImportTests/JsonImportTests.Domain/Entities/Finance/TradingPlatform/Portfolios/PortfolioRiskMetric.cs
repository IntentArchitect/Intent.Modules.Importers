using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class PortfolioRiskMetric
    {
        public PortfolioRiskMetric()
        {
            RiskRating = null!;
            ConcentrationRisk = null!;
            LiquidityRisk = null!;
        }

        public decimal VaR95 { get; set; }

        public decimal ConditionalVaR { get; set; }

        public string RiskRating { get; set; }

        public string ConcentrationRisk { get; set; }

        public string LiquidityRisk { get; set; }
    }
}