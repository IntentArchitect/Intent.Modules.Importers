using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface ICategoryMetadataDocument
    {
        IMetadataSEODocument SEO { get; }
        IDisplaySettingDocument DisplaySettings { get; }
    }
}