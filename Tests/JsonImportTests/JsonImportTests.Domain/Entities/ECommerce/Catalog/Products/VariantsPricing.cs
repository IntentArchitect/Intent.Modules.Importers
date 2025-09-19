using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class VariantsPricing
    {
        public decimal BasePrice { get; set; }

        public decimal CompareAtPrice { get; set; }

        public decimal CostPrice { get; set; }
    }
}