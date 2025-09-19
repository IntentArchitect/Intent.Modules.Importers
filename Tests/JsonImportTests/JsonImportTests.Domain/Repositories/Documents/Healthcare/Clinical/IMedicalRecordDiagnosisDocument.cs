using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IMedicalRecordDiagnosisDocument
    {
        string Id { get; }
        Guid DiagnosisId { get; }
        string Code { get; }
        string Description { get; }
        string Type { get; }
        string Severity { get; }
        DateTime OnsetDate { get; }
        string Status { get; }
    }
}