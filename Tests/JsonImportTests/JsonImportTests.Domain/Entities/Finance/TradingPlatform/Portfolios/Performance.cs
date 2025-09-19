using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class Performance
    {
        public Performance()
        {
            Volatility = null!;
        }

        public decimal TotalValue { get; set; }

        public decimal CashValue { get; set; }

        public decimal InvestedValue { get; set; }

        public decimal TotalGainLoss { get; set; }

        public decimal TotalGainLossPercentage { get; set; }

        public decimal DayChange { get; set; }

        public decimal DayChangePercentage { get; set; }

        public decimal YearToDateReturn { get; set; }

        public decimal OneYearReturn { get; set; }

        public decimal ThreeYearReturn { get; set; }

        public decimal FiveYearReturn { get; set; }

        public decimal InceptionReturn { get; set; }

        public PerformanceVolatility Volatility { get; set; }
    }
}