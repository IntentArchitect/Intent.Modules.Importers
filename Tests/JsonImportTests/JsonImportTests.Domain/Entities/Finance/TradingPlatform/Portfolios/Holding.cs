using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class Holding
    {
        private Guid? _id;

        public Holding()
        {
            Symbol = null!;
            AssetType = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public Guid AssetId { get; set; }

        public string Symbol { get; set; }

        public string AssetType { get; set; }

        public decimal Quantity { get; set; }

        public decimal AveragePrice { get; set; }

        public decimal CurrentPrice { get; set; }

        public decimal MarketValue { get; set; }

        public decimal UnrealizedGainLoss { get; set; }

        public decimal PercentageGainLoss { get; set; }

        public decimal PercentOfPortfolio { get; set; }

        public DateTime FirstPurchaseDate { get; set; }

        public DateTime LastTransactionDate { get; set; }

        public decimal DividendYield { get; set; }

        public decimal AccruedDividends { get; set; }
    }
}