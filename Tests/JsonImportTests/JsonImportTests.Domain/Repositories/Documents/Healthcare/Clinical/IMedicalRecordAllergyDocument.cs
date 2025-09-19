using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IMedicalRecordAllergyDocument
    {
        string Id { get; }
        Guid AllergyId { get; }
        string Substance { get; }
        string AllergyType { get; }
        string Severity { get; }
        string Reaction { get; }
        DateTime OnsetDate { get; }
        DateTime VerifiedDate { get; }
    }
}