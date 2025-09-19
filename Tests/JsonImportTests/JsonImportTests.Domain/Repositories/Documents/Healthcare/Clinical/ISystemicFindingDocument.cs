using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface ISystemicFindingDocument
    {
        string Head { get; }
        string Neck { get; }
        string Chest { get; }
        string Abdomen { get; }
        string Extremities { get; }
    }
}