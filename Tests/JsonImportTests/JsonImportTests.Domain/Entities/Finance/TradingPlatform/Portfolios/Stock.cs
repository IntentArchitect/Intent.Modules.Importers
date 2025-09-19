using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class Stock
    {
        public decimal Percentage { get; set; }

        public decimal Value { get; set; }

        public ICollection<Sector> Sectors { get; set; } = [];
    }
}