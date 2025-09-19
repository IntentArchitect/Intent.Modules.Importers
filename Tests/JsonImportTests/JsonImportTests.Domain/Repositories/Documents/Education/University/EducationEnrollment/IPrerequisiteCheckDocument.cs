using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IPrerequisiteCheckDocument
    {
        string Id { get; }
        Guid RequiredCourseId { get; }
        string RequiredCourse { get; }
        string MinimumGrade { get; }
        string StudentGrade { get; }
        bool IsMet { get; }
        DateTime CheckedDate { get; }
    }
}