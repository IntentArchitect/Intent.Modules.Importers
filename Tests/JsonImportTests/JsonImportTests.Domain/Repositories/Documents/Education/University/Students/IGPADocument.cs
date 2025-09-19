using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IGPADocument
    {
        decimal CumulativeGPA { get; }
        decimal SemesterGPA { get; }
        decimal MajorGPA { get; }
        decimal TotalCreditHours { get; }
        decimal QualityPoints { get; }
        decimal ClassRank { get; }
        decimal ClassSize { get; }
    }
}