using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface ISymptomDocument
    {
        string Id { get; }
        string Description { get; }
        string Severity { get; }
        string Duration { get; }
        DateTime OnsetDate { get; }
    }
}