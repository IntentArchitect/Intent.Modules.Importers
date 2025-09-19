using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Counterparty
    {
        public Counterparty()
        {
            BrokerId = null!;
            BrokerName = null!;
            DealerId = null!;
            DealerName = null!;
            PrimeBroker = null!;
        }

        public string BrokerId { get; set; }

        public string BrokerName { get; set; }

        public string DealerId { get; set; }

        public string DealerName { get; set; }

        public string PrimeBroker { get; set; }
    }
}