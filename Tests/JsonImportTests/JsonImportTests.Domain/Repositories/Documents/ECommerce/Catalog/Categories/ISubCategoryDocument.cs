using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface ISubCategoryDocument
    {
        Guid Id { get; }
        string Name { get; }
        string Slug { get; }
        decimal ProductCount { get; }
        bool IsActive { get; }
    }
}