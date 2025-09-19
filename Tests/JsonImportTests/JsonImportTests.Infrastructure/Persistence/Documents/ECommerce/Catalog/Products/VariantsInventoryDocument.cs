using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class VariantsInventoryDocument : IVariantsInventoryDocument
    {
        public decimal Quantity { get; set; }
        public decimal ReservedQuantity { get; set; }
        public decimal MinStockLevel { get; set; }
        public decimal MaxStockLevel { get; set; }
        public decimal ReorderPoint { get; set; }
        public decimal ReorderQuantity { get; set; }

        public VariantsInventory ToEntity(VariantsInventory? entity = default)
        {
            entity ??= new VariantsInventory();

            entity.Quantity = Quantity;
            entity.ReservedQuantity = ReservedQuantity;
            entity.MinStockLevel = MinStockLevel;
            entity.MaxStockLevel = MaxStockLevel;
            entity.ReorderPoint = ReorderPoint;
            entity.ReorderQuantity = ReorderQuantity;

            return entity;
        }

        public VariantsInventoryDocument PopulateFromEntity(VariantsInventory entity)
        {
            Quantity = entity.Quantity;
            ReservedQuantity = entity.ReservedQuantity;
            MinStockLevel = entity.MinStockLevel;
            MaxStockLevel = entity.MaxStockLevel;
            ReorderPoint = entity.ReorderPoint;
            ReorderQuantity = entity.ReorderQuantity;

            return this;
        }

        public static VariantsInventoryDocument? FromEntity(VariantsInventory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VariantsInventoryDocument().PopulateFromEntity(entity);
        }
    }
}