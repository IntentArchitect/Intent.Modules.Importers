using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IDisabilityDocument
    {
        string Id { get; }
        string DisabilityType { get; }
        IReadOnlyList<string> AccommodationsNeeded { get; }
        DateTime ApprovedDate { get; }
    }
}