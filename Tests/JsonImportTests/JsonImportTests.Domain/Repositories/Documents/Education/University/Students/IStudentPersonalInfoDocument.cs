using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IStudentPersonalInfoDocument
    {
        string FirstName { get; }
        string LastName { get; }
        string MiddleName { get; }
        DateTime DateOfBirth { get; }
        string Gender { get; }
        string Nationality { get; }
        string CitizenshipStatus { get; }
    }
}