using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface IAttributeValueDocument
    {
        string Id { get; }
        string Value { get; }
        string DisplayName { get; }
        string Color { get; }
        string Image { get; }
    }
}