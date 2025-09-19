using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IMeetingTimesLocationDocument
    {
        string Building { get; }
        string Room { get; }
        decimal Capacity { get; }
        IReadOnlyList<string> Equipment { get; }
    }
}