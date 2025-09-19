using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class TransactionPricing
    {
        public decimal OrderPrice { get; set; }

        public decimal ExecutedPrice { get; set; }

        public decimal GrossAmount { get; set; }

        public decimal NetAmount { get; set; }

        public decimal MarketValue { get; set; }

        public decimal AccruedInterest { get; set; }
    }
}