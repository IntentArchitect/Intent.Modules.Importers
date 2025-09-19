using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface ILearningOutcomeDocument
    {
        string Id { get; }
        Guid OutcomeId { get; }
        string Description { get; }
        string BloomLevel { get; }
        IReadOnlyList<string> AssessmentMethods { get; }
    }
}