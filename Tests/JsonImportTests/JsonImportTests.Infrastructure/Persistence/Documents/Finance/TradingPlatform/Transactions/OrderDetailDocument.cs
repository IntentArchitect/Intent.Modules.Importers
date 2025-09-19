using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class OrderDetailDocument : IOrderDetailDocument
    {
        public string OrderType { get; set; } = default!;
        public string TimeInForce { get; set; } = default!;
        public string OrderPlacedBy { get; set; } = default!;
        public string OrderChannel { get; set; } = default!;
        public DateTime OrderTime { get; set; }
        public DateTime ExecutionTime { get; set; }
        public string Venue { get; set; } = default!;
        public ExecutionQualityDocument ExecutionQuality { get; set; } = default!;
        IExecutionQualityDocument IOrderDetailDocument.ExecutionQuality => ExecutionQuality;

        public OrderDetail ToEntity(OrderDetail? entity = default)
        {
            entity ??= new OrderDetail();

            entity.OrderType = OrderType ?? throw new Exception($"{nameof(entity.OrderType)} is null");
            entity.TimeInForce = TimeInForce ?? throw new Exception($"{nameof(entity.TimeInForce)} is null");
            entity.OrderPlacedBy = OrderPlacedBy ?? throw new Exception($"{nameof(entity.OrderPlacedBy)} is null");
            entity.OrderChannel = OrderChannel ?? throw new Exception($"{nameof(entity.OrderChannel)} is null");
            entity.OrderTime = OrderTime;
            entity.ExecutionTime = ExecutionTime;
            entity.Venue = Venue ?? throw new Exception($"{nameof(entity.Venue)} is null");
            entity.ExecutionQuality = ExecutionQuality.ToEntity() ?? throw new Exception($"{nameof(entity.ExecutionQuality)} is null");

            return entity;
        }

        public OrderDetailDocument PopulateFromEntity(OrderDetail entity)
        {
            OrderType = entity.OrderType;
            TimeInForce = entity.TimeInForce;
            OrderPlacedBy = entity.OrderPlacedBy;
            OrderChannel = entity.OrderChannel;
            OrderTime = entity.OrderTime;
            ExecutionTime = entity.ExecutionTime;
            Venue = entity.Venue;
            ExecutionQuality = ExecutionQualityDocument.FromEntity(entity.ExecutionQuality)!;

            return this;
        }

        public static OrderDetailDocument? FromEntity(OrderDetail? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderDetailDocument().PopulateFromEntity(entity);
        }
    }
}