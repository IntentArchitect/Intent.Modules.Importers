using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface ICategoryAttributeDocument
    {
        string Id { get; }
        Guid AttributeId { get; }
        string Name { get; }
        string Type { get; }
        bool IsRequired { get; }
        bool IsFilterable { get; }
        IReadOnlyList<IValueDocument> Values { get; }
    }
}