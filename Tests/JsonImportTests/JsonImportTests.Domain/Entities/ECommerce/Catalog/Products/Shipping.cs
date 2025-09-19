using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class Shipping
    {
        public Shipping()
        {
            ShippingClass = null!;
            Dimensions = null!;
        }

        public decimal Weight { get; set; }

        public string ShippingClass { get; set; }

        public bool RequiresShipping { get; set; }

        public bool IsFreeShipping { get; set; }

        public decimal HandlingTime { get; set; }

        public ShippingDimension Dimensions { get; set; }
    }
}