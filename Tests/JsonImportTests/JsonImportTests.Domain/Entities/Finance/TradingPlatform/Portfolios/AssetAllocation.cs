using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class AssetAllocation
    {
        public AssetAllocation()
        {
            Stocks = null!;
            Other = null!;
            Cash = null!;
            Bonds = null!;
        }

        public Stock Stocks { get; set; }

        public Other Other { get; set; }

        public Cash Cash { get; set; }

        public Bond Bonds { get; set; }
    }
}