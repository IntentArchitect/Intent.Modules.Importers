using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Allocation
    {
        private string? _id;

        public Allocation()
        {
            Id = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid AccountId { get; set; }

        public decimal AllocationPercentage { get; set; }

        public decimal AllocatedQuantity { get; set; }

        public decimal AllocatedAmount { get; set; }
    }
}