using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IStudentContactInfoDocument
    {
        string Email { get; }
        string AlternateEmail { get; }
        string Phone { get; }
        IContactInfoEmergencyContactDocument EmergencyContact { get; }
        IStudentContactInfoAddressDocument Address { get; }
    }
}