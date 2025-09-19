using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class StockLevel
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
    }
}