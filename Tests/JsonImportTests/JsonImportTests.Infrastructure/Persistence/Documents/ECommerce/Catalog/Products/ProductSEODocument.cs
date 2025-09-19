using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ProductSEODocument : IProductSEODocument
    {
        public string MetaTitle { get; set; } = default!;
        public string MetaDescription { get; set; } = default!;
        public List<string> MetaKeywords { get; set; } = default!;
        IReadOnlyList<string> IProductSEODocument.MetaKeywords => MetaKeywords;
        public string UrlSlug { get; set; } = default!;
        public string CanonicalUrl { get; set; } = default!;

        public ProductSEO ToEntity(ProductSEO? entity = default)
        {
            entity ??= new ProductSEO();

            entity.MetaTitle = MetaTitle ?? throw new Exception($"{nameof(entity.MetaTitle)} is null");
            entity.MetaDescription = MetaDescription ?? throw new Exception($"{nameof(entity.MetaDescription)} is null");
            entity.MetaKeywords = MetaKeywords ?? throw new Exception($"{nameof(entity.MetaKeywords)} is null");
            entity.UrlSlug = UrlSlug ?? throw new Exception($"{nameof(entity.UrlSlug)} is null");
            entity.CanonicalUrl = CanonicalUrl ?? throw new Exception($"{nameof(entity.CanonicalUrl)} is null");

            return entity;
        }

        public ProductSEODocument PopulateFromEntity(ProductSEO entity)
        {
            MetaTitle = entity.MetaTitle;
            MetaDescription = entity.MetaDescription;
            MetaKeywords = entity.MetaKeywords.ToList();
            UrlSlug = entity.UrlSlug;
            CanonicalUrl = entity.CanonicalUrl;

            return this;
        }

        public static ProductSEODocument? FromEntity(ProductSEO? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductSEODocument().PopulateFromEntity(entity);
        }
    }
}