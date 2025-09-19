using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Instrument
    {
        public Instrument()
        {
            Symbol = null!;
            Name = null!;
            AssetType = null!;
            ISIN = null!;
            Exchange = null!;
            Currency = null!;
        }

        public string Symbol { get; set; }

        public string Name { get; set; }

        public string AssetType { get; set; }

        public string ISIN { get; set; }

        public string Exchange { get; set; }

        public string Currency { get; set; }
    }
}