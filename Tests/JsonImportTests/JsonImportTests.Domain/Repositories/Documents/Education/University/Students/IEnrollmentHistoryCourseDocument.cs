using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IEnrollmentHistoryCourseDocument
    {
        string Id { get; }
        Guid CourseId { get; }
        string CourseCode { get; }
        decimal CreditHours { get; }
        string Grade { get; }
        decimal GradePoints { get; }
        DateTime GradeDate { get; }
    }
}