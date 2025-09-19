using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class PerformanceVolatility
    {
        public decimal Beta { get; set; }

        public decimal StandardDeviation { get; set; }

        public decimal SharpeRatio { get; set; }

        public decimal MaxDrawdown { get; set; }
    }
}