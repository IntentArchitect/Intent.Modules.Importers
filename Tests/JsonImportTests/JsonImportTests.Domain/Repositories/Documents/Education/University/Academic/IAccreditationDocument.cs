using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IAccreditationDocument
    {
        string AccreditingBody { get; }
        string AccreditationLevel { get; }
        DateTime LastReviewDate { get; }
        DateTime NextReviewDate { get; }
        string Status { get; }
    }
}