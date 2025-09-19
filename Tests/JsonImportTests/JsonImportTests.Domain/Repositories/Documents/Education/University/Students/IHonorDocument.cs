using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IHonorDocument
    {
        Guid HonorId { get; }
        string Title { get; }
        string Organization { get; }
        DateTime DateReceived { get; }
        string Description { get; }
    }
}