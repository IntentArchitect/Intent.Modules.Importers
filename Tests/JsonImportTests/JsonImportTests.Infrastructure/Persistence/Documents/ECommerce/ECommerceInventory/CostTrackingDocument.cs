using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class CostTrackingDocument : ICostTrackingDocument
    {
        public decimal AverageCost { get; set; }
        public decimal LastCost { get; set; }
        public decimal StandardCost { get; set; }
        public decimal TotalValue { get; set; }
        public string CostMethod { get; set; } = default!;

        public CostTracking ToEntity(CostTracking? entity = default)
        {
            entity ??= new CostTracking();

            entity.AverageCost = AverageCost;
            entity.LastCost = LastCost;
            entity.StandardCost = StandardCost;
            entity.TotalValue = TotalValue;
            entity.CostMethod = CostMethod ?? throw new Exception($"{nameof(entity.CostMethod)} is null");

            return entity;
        }

        public CostTrackingDocument PopulateFromEntity(CostTracking entity)
        {
            AverageCost = entity.AverageCost;
            LastCost = entity.LastCost;
            StandardCost = entity.StandardCost;
            TotalValue = entity.TotalValue;
            CostMethod = entity.CostMethod;

            return this;
        }

        public static CostTrackingDocument? FromEntity(CostTracking? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CostTrackingDocument().PopulateFromEntity(entity);
        }
    }
}