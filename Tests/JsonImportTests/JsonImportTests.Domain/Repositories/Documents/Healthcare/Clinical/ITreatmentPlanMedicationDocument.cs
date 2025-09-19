using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface ITreatmentPlanMedicationDocument
    {
        string Id { get; }
        string Name { get; }
        string Dosage { get; }
        string Frequency { get; }
        string Duration { get; }
    }
}