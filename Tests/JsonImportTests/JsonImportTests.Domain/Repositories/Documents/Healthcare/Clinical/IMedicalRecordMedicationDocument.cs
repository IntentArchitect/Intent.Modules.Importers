using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IMedicalRecordMedicationDocument
    {
        string Id { get; }
        Guid MedicationId { get; }
        string Name { get; }
        string GenericName { get; }
        string Dosage { get; }
        string Route { get; }
        string Frequency { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        string PrescribingPhysician { get; }
        string Instructions { get; }
    }
}