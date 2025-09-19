using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients
{
    public interface IPatientEmergencyContactDocument
    {
        string Id { get; }
        string Name { get; }
        string Relationship { get; }
        string Phone { get; }
        string Email { get; }
    }
}