using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IITServiceDocument
    {
        string Id { get; }
        string ServiceType { get; }
        bool IsActive { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}