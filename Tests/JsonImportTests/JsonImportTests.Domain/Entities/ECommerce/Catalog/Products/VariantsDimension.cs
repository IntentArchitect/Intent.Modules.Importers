using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class VariantsDimension
    {
        public VariantsDimension()
        {
            Unit = null!;
        }

        public decimal Length { get; set; }

        public decimal Width { get; set; }

        public decimal Height { get; set; }

        public decimal Weight { get; set; }

        public string Unit { get; set; }
    }
}