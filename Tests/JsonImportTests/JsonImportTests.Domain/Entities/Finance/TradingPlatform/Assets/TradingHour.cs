using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class TradingHour
    {
        public TradingHour()
        {
            MarketOpen = null!;
            MarketClose = null!;
            TimeZone = null!;
        }

        public string MarketOpen { get; set; }

        public string MarketClose { get; set; }

        public string TimeZone { get; set; }
    }
}