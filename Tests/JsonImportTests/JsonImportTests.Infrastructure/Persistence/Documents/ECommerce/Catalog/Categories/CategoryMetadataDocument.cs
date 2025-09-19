using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class CategoryMetadataDocument : ICategoryMetadataDocument
    {
        public MetadataSEODocument SEO { get; set; } = default!;
        IMetadataSEODocument ICategoryMetadataDocument.SEO => SEO;
        public DisplaySettingDocument DisplaySettings { get; set; } = default!;
        IDisplaySettingDocument ICategoryMetadataDocument.DisplaySettings => DisplaySettings;

        public CategoryMetadata ToEntity(CategoryMetadata? entity = default)
        {
            entity ??= new CategoryMetadata();
            entity.SEO = SEO.ToEntity() ?? throw new Exception($"{nameof(entity.SEO)} is null");
            entity.DisplaySettings = DisplaySettings.ToEntity() ?? throw new Exception($"{nameof(entity.DisplaySettings)} is null");

            return entity;
        }

        public CategoryMetadataDocument PopulateFromEntity(CategoryMetadata entity)
        {
            SEO = MetadataSEODocument.FromEntity(entity.SEO)!;
            DisplaySettings = DisplaySettingDocument.FromEntity(entity.DisplaySettings)!;

            return this;
        }

        public static CategoryMetadataDocument? FromEntity(CategoryMetadata? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CategoryMetadataDocument().PopulateFromEntity(entity);
        }
    }
}