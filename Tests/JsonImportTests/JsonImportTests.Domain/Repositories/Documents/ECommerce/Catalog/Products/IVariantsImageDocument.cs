using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IVariantsImageDocument
    {
        Guid Id { get; }
        string Url { get; }
        string AltText { get; }
        decimal Position { get; }
        bool IsPrimary { get; }
    }
}