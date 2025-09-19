using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class CategoryMetadata
    {
        public CategoryMetadata()
        {
            SEO = null!;
            DisplaySettings = null!;
        }

        public MetadataSEO SEO { get; set; }

        public DisplaySetting DisplaySettings { get; set; }
    }
}