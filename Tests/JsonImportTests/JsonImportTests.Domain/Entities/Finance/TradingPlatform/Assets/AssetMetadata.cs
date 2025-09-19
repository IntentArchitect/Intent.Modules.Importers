using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets
{
    public class AssetMetadata
    {
        public AssetMetadata()
        {
            DataProvider = null!;
            DataQuality = null!;
            DelistingDate = null!;
        }

        public string DataProvider { get; set; }

        public DateTime LastUpdated { get; set; }

        public string DataQuality { get; set; }

        public bool IsActive { get; set; }

        public DateTime ListingDate { get; set; }

        public object DelistingDate { get; set; }
    }
}