using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IProductAttributeDocument
    {
        string Id { get; }
        string Name { get; }
        string Value { get; }
        string Type { get; }
        bool IsFilterable { get; }
        bool IsSearchable { get; }
        decimal DisplayOrder { get; }
    }
}