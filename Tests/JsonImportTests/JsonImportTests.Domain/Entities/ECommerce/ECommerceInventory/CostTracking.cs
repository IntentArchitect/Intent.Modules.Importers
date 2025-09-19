using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class CostTracking
    {
        public CostTracking()
        {
            CostMethod = null!;
        }

        public decimal AverageCost { get; set; }

        public decimal LastCost { get; set; }

        public decimal StandardCost { get; set; }

        public decimal TotalValue { get; set; }

        public string CostMethod { get; set; }
    }
}