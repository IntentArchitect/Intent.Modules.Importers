using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class ProductPricing
    {
        public ProductPricing()
        {
            Currency = null!;
            TaxClass = null!;
        }

        public decimal BasePrice { get; set; }

        public decimal CompareAtPrice { get; set; }

        public decimal CostPrice { get; set; }

        public string Currency { get; set; }

        public string TaxClass { get; set; }

        public ICollection<DiscountRule> DiscountRules { get; set; } = [];
    }
}