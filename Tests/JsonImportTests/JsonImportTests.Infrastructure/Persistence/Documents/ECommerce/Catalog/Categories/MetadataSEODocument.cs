using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class MetadataSEODocument : IMetadataSEODocument
    {
        public string MetaTitle { get; set; } = default!;
        public string MetaDescription { get; set; } = default!;
        public List<string> MetaKeywords { get; set; } = default!;
        IReadOnlyList<string> IMetadataSEODocument.MetaKeywords => MetaKeywords;
        public string CanonicalUrl { get; set; } = default!;

        public MetadataSEO ToEntity(MetadataSEO? entity = default)
        {
            entity ??= new MetadataSEO();

            entity.MetaTitle = MetaTitle ?? throw new Exception($"{nameof(entity.MetaTitle)} is null");
            entity.MetaDescription = MetaDescription ?? throw new Exception($"{nameof(entity.MetaDescription)} is null");
            entity.MetaKeywords = MetaKeywords ?? throw new Exception($"{nameof(entity.MetaKeywords)} is null");
            entity.CanonicalUrl = CanonicalUrl ?? throw new Exception($"{nameof(entity.CanonicalUrl)} is null");

            return entity;
        }

        public MetadataSEODocument PopulateFromEntity(MetadataSEO entity)
        {
            MetaTitle = entity.MetaTitle;
            MetaDescription = entity.MetaDescription;
            MetaKeywords = entity.MetaKeywords.ToList();
            CanonicalUrl = entity.CanonicalUrl;

            return this;
        }

        public static MetadataSEODocument? FromEntity(MetadataSEO? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MetadataSEODocument().PopulateFromEntity(entity);
        }
    }
}