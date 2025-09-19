using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface ISemesterDocument
    {
        Guid SemesterId { get; }
        string SemesterName { get; }
        decimal Year { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        bool IsCurrentSemester { get; }
    }
}