using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface ITextBookDocument
    {
        string Id { get; }
        string ISBN { get; }
        string Title { get; }
        string Author { get; }
        string Edition { get; }
        string Publisher { get; }
        bool IsRequired { get; }
    }
}