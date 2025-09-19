using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IAssignmentDocument
    {
        Guid AssignmentId { get; }
        string Title { get; }
        string Type { get; }
        DateTime DueDate { get; }
        decimal Points { get; }
        string Instructions { get; }
    }
}