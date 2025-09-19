using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IOnlineResourceDocument
    {
        string Id { get; }
        string Title { get; }
        string URL { get; }
        string Type { get; }
        bool AccessRequired { get; }
    }
}