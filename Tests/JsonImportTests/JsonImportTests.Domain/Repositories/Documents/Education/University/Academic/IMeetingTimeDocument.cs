using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IMeetingTimeDocument
    {
        string Id { get; }
        string Day { get; }
        string StartTime { get; }
        string EndTime { get; }
        IMeetingTimesLocationDocument Location { get; }
    }
}