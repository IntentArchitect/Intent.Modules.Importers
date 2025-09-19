using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class VariantsInventory
    {
        public decimal Quantity { get; set; }

        public decimal ReservedQuantity { get; set; }

        public decimal MinStockLevel { get; set; }

        public decimal MaxStockLevel { get; set; }

        public decimal ReorderPoint { get; set; }

        public decimal ReorderQuantity { get; set; }
    }
}