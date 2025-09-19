using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients
{
    public interface IPatientContactInfoDocument
    {
        string PrimaryPhone { get; }
        string SecondaryPhone { get; }
        string Email { get; }
        IPatientContactInfoAddressDocument Address { get; }
    }
}