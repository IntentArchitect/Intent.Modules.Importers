using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class AssetMetadataDocument : IAssetMetadataDocument
    {
        public string DataProvider { get; set; } = default!;
        public DateTime LastUpdated { get; set; }
        public string DataQuality { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime ListingDate { get; set; }
        public object DelistingDate { get; set; } = default!;

        public AssetMetadata ToEntity(AssetMetadata? entity = default)
        {
            entity ??= new AssetMetadata();

            entity.DataProvider = DataProvider ?? throw new Exception($"{nameof(entity.DataProvider)} is null");
            entity.LastUpdated = LastUpdated;
            entity.DataQuality = DataQuality ?? throw new Exception($"{nameof(entity.DataQuality)} is null");
            entity.IsActive = IsActive;
            entity.ListingDate = ListingDate;
            entity.DelistingDate = DelistingDate ?? throw new Exception($"{nameof(entity.DelistingDate)} is null");

            return entity;
        }

        public AssetMetadataDocument PopulateFromEntity(AssetMetadata entity)
        {
            DataProvider = entity.DataProvider;
            LastUpdated = entity.LastUpdated;
            DataQuality = entity.DataQuality;
            IsActive = entity.IsActive;
            ListingDate = entity.ListingDate;
            DelistingDate = entity.DelistingDate;

            return this;
        }

        public static AssetMetadataDocument? FromEntity(AssetMetadata? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssetMetadataDocument().PopulateFromEntity(entity);
        }
    }
}