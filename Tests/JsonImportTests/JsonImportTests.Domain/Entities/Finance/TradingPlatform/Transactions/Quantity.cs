using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Quantity
    {
        public decimal OrderedQuantity { get; set; }

        public decimal ExecutedQuantity { get; set; }

        public decimal RemainingQuantity { get; set; }

        public decimal CancelledQuantity { get; set; }
    }
}