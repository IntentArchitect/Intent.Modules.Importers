using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface IMetadataSEODocument
    {
        string MetaTitle { get; }
        string MetaDescription { get; }
        IReadOnlyList<string> MetaKeywords { get; }
        string CanonicalUrl { get; }
    }
}