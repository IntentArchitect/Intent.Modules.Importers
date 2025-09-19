using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IRubricDocument
    {
        string Id { get; }
        Guid CriteriaId { get; }
        string CriteriaName { get; }
        decimal MaxPoints { get; }
        decimal EarnedPoints { get; }
        string Comments { get; }
    }
}