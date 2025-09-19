using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class Analyst
    {
        public Analyst()
        {
            Consensus = null!;
            Recommendations = null!;
        }

        public string Consensus { get; set; }

        public decimal TargetPrice { get; set; }

        public decimal PriceTargetHigh { get; set; }

        public decimal PriceTargetLow { get; set; }

        public Recommendation Recommendations { get; set; }
    }
}