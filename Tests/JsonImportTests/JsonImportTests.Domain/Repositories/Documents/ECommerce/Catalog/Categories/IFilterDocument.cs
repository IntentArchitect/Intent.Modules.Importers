using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface IFilterDocument
    {
        string Id { get; }
        string Name { get; }
        string Type { get; }
        IReadOnlyList<IOptionDocument> Options { get; }
    }
}