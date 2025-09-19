using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IEnrollmentStudentDocument
    {
        Guid StudentId { get; }
        string StudentNumber { get; }
        string FirstName { get; }
        string LastName { get; }
        string Email { get; }
        string Program { get; }
        string ClassLevel { get; }
    }
}