using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class QuantityDocument : IQuantityDocument
    {
        public decimal OrderedQuantity { get; set; }
        public decimal ExecutedQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal CancelledQuantity { get; set; }

        public Quantity ToEntity(Quantity? entity = default)
        {
            entity ??= new Quantity();

            entity.OrderedQuantity = OrderedQuantity;
            entity.ExecutedQuantity = ExecutedQuantity;
            entity.RemainingQuantity = RemainingQuantity;
            entity.CancelledQuantity = CancelledQuantity;

            return entity;
        }

        public QuantityDocument PopulateFromEntity(Quantity entity)
        {
            OrderedQuantity = entity.OrderedQuantity;
            ExecutedQuantity = entity.ExecutedQuantity;
            RemainingQuantity = entity.RemainingQuantity;
            CancelledQuantity = entity.CancelledQuantity;

            return this;
        }

        public static QuantityDocument? FromEntity(Quantity? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new QuantityDocument().PopulateFromEntity(entity);
        }
    }
}