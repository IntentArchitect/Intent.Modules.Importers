using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class StockLevelDocument : IStockLevelDocument
    {
        public decimal Available { get; set; }
        public decimal Reserved { get; set; }
        public decimal OnOrder { get; set; }
        public decimal Damaged { get; set; }
        public decimal Total { get; set; }
        public decimal MinimumLevel { get; set; }
        public decimal MaximumLevel { get; set; }
        public decimal ReorderPoint { get; set; }
        public decimal ReorderQuantity { get; set; }

        public StockLevel ToEntity(StockLevel? entity = default)
        {
            entity ??= new StockLevel();

            entity.Available = Available;
            entity.Reserved = Reserved;
            entity.OnOrder = OnOrder;
            entity.Damaged = Damaged;
            entity.Total = Total;
            entity.MinimumLevel = MinimumLevel;
            entity.MaximumLevel = MaximumLevel;
            entity.ReorderPoint = ReorderPoint;
            entity.ReorderQuantity = ReorderQuantity;

            return entity;
        }

        public StockLevelDocument PopulateFromEntity(StockLevel entity)
        {
            Available = entity.Available;
            Reserved = entity.Reserved;
            OnOrder = entity.OnOrder;
            Damaged = entity.Damaged;
            Total = entity.Total;
            MinimumLevel = entity.MinimumLevel;
            MaximumLevel = entity.MaximumLevel;
            ReorderPoint = entity.ReorderPoint;
            ReorderQuantity = entity.ReorderQuantity;

            return this;
        }

        public static StockLevelDocument? FromEntity(StockLevel? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new StockLevelDocument().PopulateFromEntity(entity);
        }
    }
}