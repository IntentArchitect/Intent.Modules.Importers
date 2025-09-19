using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IEnrollmentCourseDocument
    {
        Guid CourseId { get; }
        string CourseCode { get; }
        string CourseName { get; }
        decimal CreditHours { get; }
        string Department { get; }
    }
}