using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface ISportDocument
    {
        Guid SportId { get; }
        string SportName { get; }
        string Position { get; }
        string Season { get; }
        decimal Year { get; }
        decimal ScholarshipAmount { get; }
    }
}