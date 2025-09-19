using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class OrderDetail
    {
        public OrderDetail()
        {
            OrderType = null!;
            TimeInForce = null!;
            OrderPlacedBy = null!;
            OrderChannel = null!;
            Venue = null!;
            ExecutionQuality = null!;
        }

        public string OrderType { get; set; }

        public string TimeInForce { get; set; }

        public string OrderPlacedBy { get; set; }

        public string OrderChannel { get; set; }

        public DateTime OrderTime { get; set; }

        public DateTime ExecutionTime { get; set; }

        public string Venue { get; set; }

        public ExecutionQuality ExecutionQuality { get; set; }
    }
}