using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IVisitDetailsDiagnosisDocument
    {
        string Id { get; }
        string Code { get; }
        string Description { get; }
        string Type { get; }
        string Certainty { get; }
    }
}