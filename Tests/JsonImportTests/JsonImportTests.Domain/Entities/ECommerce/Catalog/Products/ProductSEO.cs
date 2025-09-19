using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.Catalog.Products
{
    public class ProductSEO
    {
        public ProductSEO()
        {
            MetaTitle = null!;
            MetaDescription = null!;
            UrlSlug = null!;
            CanonicalUrl = null!;
        }

        public string MetaTitle { get; set; }

        public string MetaDescription { get; set; }

        public IList<string> MetaKeywords { get; set; } = [];

        public string UrlSlug { get; set; }

        public string CanonicalUrl { get; set; }
    }
}