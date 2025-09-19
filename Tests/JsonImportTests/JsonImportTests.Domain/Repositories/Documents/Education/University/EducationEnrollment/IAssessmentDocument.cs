using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IAssessmentDocument
    {
        Guid AssessmentId { get; }
        string Type { get; }
        string Name { get; }
        decimal MaxPoints { get; }
        decimal EarnedPoints { get; }
        decimal Percentage { get; }
        decimal Weight { get; }
        DateTime DueDate { get; }
        DateTime SubmittedDate { get; }
        DateTime GradedDate { get; }
        string Comments { get; }
        IReadOnlyList<IRubricDocument> Rubric { get; }
    }
}