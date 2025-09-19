using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class InventoryItemLocationDocument : IInventoryItemLocationDocument
    {
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; } = default!;
        public string Zone { get; set; } = default!;
        public string Aisle { get; set; } = default!;
        public string Shelf { get; set; } = default!;
        public string Bin { get; set; } = default!;

        public InventoryItemLocation ToEntity(InventoryItemLocation? entity = default)
        {
            entity ??= new InventoryItemLocation();

            entity.WarehouseId = WarehouseId;
            entity.WarehouseName = WarehouseName ?? throw new Exception($"{nameof(entity.WarehouseName)} is null");
            entity.Zone = Zone ?? throw new Exception($"{nameof(entity.Zone)} is null");
            entity.Aisle = Aisle ?? throw new Exception($"{nameof(entity.Aisle)} is null");
            entity.Shelf = Shelf ?? throw new Exception($"{nameof(entity.Shelf)} is null");
            entity.Bin = Bin ?? throw new Exception($"{nameof(entity.Bin)} is null");

            return entity;
        }

        public InventoryItemLocationDocument PopulateFromEntity(InventoryItemLocation entity)
        {
            WarehouseId = entity.WarehouseId;
            WarehouseName = entity.WarehouseName;
            Zone = entity.Zone;
            Aisle = entity.Aisle;
            Shelf = entity.Shelf;
            Bin = entity.Bin;

            return this;
        }

        public static InventoryItemLocationDocument? FromEntity(InventoryItemLocation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InventoryItemLocationDocument().PopulateFromEntity(entity);
        }
    }
}