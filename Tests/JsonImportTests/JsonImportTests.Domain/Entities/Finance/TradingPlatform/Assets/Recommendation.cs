using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class Recommendation
    {
        public decimal StrongBuy { get; set; }

        public decimal Buy { get; set; }

        public decimal Hold { get; set; }

        public decimal Sell { get; set; }

        public decimal StrongSell { get; set; }
    }
}