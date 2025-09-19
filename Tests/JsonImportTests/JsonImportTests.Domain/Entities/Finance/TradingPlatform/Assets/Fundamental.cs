using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class Fundamental
    {
        public decimal MarketCapitalization { get; set; }

        public decimal EnterpriseValue { get; set; }

        public decimal PERatio { get; set; }

        public decimal EarningsPerShare { get; set; }

        public decimal BookValuePerShare { get; set; }

        public decimal PriceToBook { get; set; }

        public decimal PriceToSales { get; set; }

        public decimal DividendPerShare { get; set; }

        public decimal DividendYield { get; set; }

        public decimal PayoutRatio { get; set; }

        public decimal ReturnOnEquity { get; set; }

        public decimal ReturnOnAssets { get; set; }

        public decimal DebtToEquity { get; set; }

        public decimal CurrentRatio { get; set; }

        public decimal QuickRatio { get; set; }
    }
}