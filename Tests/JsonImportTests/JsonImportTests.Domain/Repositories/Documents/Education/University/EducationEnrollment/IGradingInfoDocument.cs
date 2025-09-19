using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IGradingInfoDocument
    {
        string CurrentGrade { get; }
        decimal CurrentPercentage { get; }
        decimal GradePoints { get; }
        decimal QualityPoints { get; }
        bool IsPassFail { get; }
        object IncompleteReason { get; }
        IReadOnlyList<IGradeChangeHistoryDocument> GradeChangeHistory { get; }
    }
}