using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface IDisplaySettingDocument
    {
        string BannerImage { get; }
        string IconUrl { get; }
        string Color { get; }
        string LayoutTemplate { get; }
    }
}