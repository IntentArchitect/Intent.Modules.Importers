using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes
{
    public interface ITimestampDocument
    {
        DateTime CreatedAt { get; }
        DateTime UpdatedAt { get; }
        object LastAccessedAt { get; }
        DateTime ScheduledAt { get; }
        object CompletedAt { get; }
        object DeletedAt { get; }
    }
}