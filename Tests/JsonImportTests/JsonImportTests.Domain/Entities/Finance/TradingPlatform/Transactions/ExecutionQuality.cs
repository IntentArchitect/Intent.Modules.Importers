using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class ExecutionQuality
    {
        public decimal PriceImprovement { get; set; }

        public decimal SpreadCaptured { get; set; }

        public decimal TimeToExecution { get; set; }
    }
}