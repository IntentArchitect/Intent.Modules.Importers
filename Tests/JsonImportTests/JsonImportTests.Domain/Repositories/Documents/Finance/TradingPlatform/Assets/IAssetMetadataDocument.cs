using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets
{
    public interface IAssetMetadataDocument
    {
        string DataProvider { get; }
        DateTime LastUpdated { get; }
        string DataQuality { get; }
        bool IsActive { get; }
        DateTime ListingDate { get; }
        object DelistingDate { get; }
    }
}