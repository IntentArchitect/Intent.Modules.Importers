using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class PricingDatum
    {
        public PricingDatum()
        {
            Volatility = null!;
        }

        public decimal CurrentPrice { get; set; }

        public decimal PreviousClose { get; set; }

        public decimal DayHigh { get; set; }

        public decimal DayLow { get; set; }

        public decimal WeekHigh52 { get; set; }

        public decimal WeekLow52 { get; set; }

        public decimal Volume { get; set; }

        public decimal AverageVolume10Day { get; set; }

        public decimal AverageVolume3Month { get; set; }

        public decimal Beta { get; set; }

        public PricingDataVolatility Volatility { get; set; }
    }
}