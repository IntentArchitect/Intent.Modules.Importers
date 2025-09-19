using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class DisplaySettingDocument : IDisplaySettingDocument
    {
        public string BannerImage { get; set; } = default!;
        public string IconUrl { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string LayoutTemplate { get; set; } = default!;

        public DisplaySetting ToEntity(DisplaySetting? entity = default)
        {
            entity ??= new DisplaySetting();

            entity.BannerImage = BannerImage ?? throw new Exception($"{nameof(entity.BannerImage)} is null");
            entity.IconUrl = IconUrl ?? throw new Exception($"{nameof(entity.IconUrl)} is null");
            entity.Color = Color ?? throw new Exception($"{nameof(entity.Color)} is null");
            entity.LayoutTemplate = LayoutTemplate ?? throw new Exception($"{nameof(entity.LayoutTemplate)} is null");

            return entity;
        }

        public DisplaySettingDocument PopulateFromEntity(DisplaySetting entity)
        {
            BannerImage = entity.BannerImage;
            IconUrl = entity.IconUrl;
            Color = entity.Color;
            LayoutTemplate = entity.LayoutTemplate;

            return this;
        }

        public static DisplaySettingDocument? FromEntity(DisplaySetting? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DisplaySettingDocument().PopulateFromEntity(entity);
        }
    }
}