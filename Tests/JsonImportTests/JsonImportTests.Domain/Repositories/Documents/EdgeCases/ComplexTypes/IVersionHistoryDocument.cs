using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface IVersionHistoryDocument
    {
        string Id { get; }
        string Version { get; }
        IReadOnlyList<string> Changes { get; }
        DateTime Date { get; }
        string Author { get; }
        bool Breaking { get; }
    }
}