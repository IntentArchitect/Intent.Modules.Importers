using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class TradingInfo
    {
        public TradingInfo()
        {
            PrimaryExchange = null!;
            TradingHours = null!;
        }

        public string PrimaryExchange { get; set; }

        public IList<string> SecondaryExchanges { get; set; } = [];

        public decimal TickSize { get; set; }

        public decimal LotSize { get; set; }

        public bool IsShortable { get; set; }

        public bool IsMarginable { get; set; }

        public bool DividendEligible { get; set; }

        public TradingHour TradingHours { get; set; }
    }
}