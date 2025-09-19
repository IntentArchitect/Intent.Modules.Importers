using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class Restriction
    {
        public decimal MaxPositionSize { get; set; }

        public IList<string> ProhibitedAssets { get; set; } = [];

        public decimal MinCashPercentage { get; set; }

        public decimal MaxSectorConcentration { get; set; }
    }
}