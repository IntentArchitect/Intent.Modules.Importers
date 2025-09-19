using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients
{
    public interface IMedicalHistoryAllergyDocument
    {
        string Id { get; }
        string Substance { get; }
        string Severity { get; }
        string Reaction { get; }
    }
}