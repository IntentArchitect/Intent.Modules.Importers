using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IInterventionDocument
    {
        string Id { get; }
        string Type { get; }
        string Description { get; }
        string Frequency { get; }
        string Duration { get; }
    }
}