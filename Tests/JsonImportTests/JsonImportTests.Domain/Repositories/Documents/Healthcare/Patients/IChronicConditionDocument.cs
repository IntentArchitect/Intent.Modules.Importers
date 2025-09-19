using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients
{
    public interface IChronicConditionDocument
    {
        string Id { get; }
        string Condition { get; }
        DateTime DiagnosedDate { get; }
        string Status { get; }
    }
}