using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories
{
    public class MetadataSEO
    {
        public MetadataSEO()
        {
            MetaTitle = null!;
            MetaDescription = null!;
            CanonicalUrl = null!;
        }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public IList<string> MetaKeywords { get; set; } = [];

        public string CanonicalUrl { get; set; }
    }
}