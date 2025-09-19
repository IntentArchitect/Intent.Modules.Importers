using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class VariantsAttribute
    {
        public VariantsAttribute()
        {
            Color = null!;
            Size = null!;
            Material = null!;
        }

        public string Color { get; set; }

        public string Size { get; set; }

        public string Material { get; set; }

        public decimal Weight { get; set; }
    }
}