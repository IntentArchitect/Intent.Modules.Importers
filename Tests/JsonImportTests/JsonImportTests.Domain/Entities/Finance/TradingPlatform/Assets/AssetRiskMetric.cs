using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class AssetRiskMetric
    {
        public AssetRiskMetric()
        {
            LiquidityRisk = null!;
            ConcentrationRisk = null!;
            RegulatoryRisk = null!;
            CreditRating = null!;
        }

        public decimal ESGScore { get; set; }

        public string LiquidityRisk { get; set; }

        public string ConcentrationRisk { get; set; }

        public string RegulatoryRisk { get; set; }

        public CreditRating CreditRating { get; set; }
    }
}