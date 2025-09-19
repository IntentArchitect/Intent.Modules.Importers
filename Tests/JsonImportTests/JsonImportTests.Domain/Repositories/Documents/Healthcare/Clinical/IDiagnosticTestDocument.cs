using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IDiagnosticTestDocument
    {
        string Id { get; }
        Guid TestId { get; }
        string TestName { get; }
        string TestType { get; }
        DateTime OrderedDate { get; }
        DateTime CompletedDate { get; }
        string Results { get; }
        string Interpretation { get; }
        string OrderingPhysician { get; }
    }
}