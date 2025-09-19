using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IEnrollmentHistoryDocument
    {
        string Id { get; }
        Guid SemesterId { get; }
        string Semester { get; }
        decimal Year { get; }
        string EnrollmentStatus { get; }
        decimal CreditHours { get; }
        decimal SemesterGPA { get; }
        bool DeansList { get; }
        bool Probation { get; }
        IReadOnlyList<IEnrollmentHistoryCourseDocument> Courses { get; }
    }
}