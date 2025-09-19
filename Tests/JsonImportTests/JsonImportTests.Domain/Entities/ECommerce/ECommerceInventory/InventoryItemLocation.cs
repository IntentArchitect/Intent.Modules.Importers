using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class InventoryItemLocation
    {
        public InventoryItemLocation()
        {
            WarehouseName = null!;
            Zone = null!;
            Aisle = null!;
            Shelf = null!;
            Bin = null!;
        }

        public Guid WarehouseId { get; set; }

        public string WarehouseName { get; set; }

        public string Zone { get; set; }

        public string Aisle { get; set; }

        public string Shelf { get; set; }

        public string Bin { get; set; }
    }
}