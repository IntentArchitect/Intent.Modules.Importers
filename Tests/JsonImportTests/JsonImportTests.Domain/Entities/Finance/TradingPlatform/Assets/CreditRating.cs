using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class CreditRating
    {
        public CreditRating()
        {
            SP = null!;
            Moody = null!;
            Fitch = null!;
        }

        public string SP { get; set; }

        public string Moody { get; set; }

        public string Fitch { get; set; }
    }
}