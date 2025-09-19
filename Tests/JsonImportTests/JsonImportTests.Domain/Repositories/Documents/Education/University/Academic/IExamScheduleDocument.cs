using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IExamScheduleDocument
    {
        DateTime MidtermDate { get; }
        DateTime FinalExamDate { get; }
        string Location { get; }
    }
}