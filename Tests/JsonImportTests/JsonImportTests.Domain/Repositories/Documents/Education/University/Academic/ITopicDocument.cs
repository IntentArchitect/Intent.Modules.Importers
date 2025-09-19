using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface ITopicDocument
    {
        string Id { get; }
        string Title { get; }
        string Description { get; }
        decimal EstimatedHours { get; }
    }
}