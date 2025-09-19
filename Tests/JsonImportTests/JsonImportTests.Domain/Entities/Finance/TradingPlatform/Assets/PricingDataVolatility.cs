using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class PricingDataVolatility
    {
        public decimal ImpliedVolatility { get; set; }

        public decimal HistoricalVolatility30Day { get; set; }

        public decimal HistoricalVolatility90Day { get; set; }
    }
}