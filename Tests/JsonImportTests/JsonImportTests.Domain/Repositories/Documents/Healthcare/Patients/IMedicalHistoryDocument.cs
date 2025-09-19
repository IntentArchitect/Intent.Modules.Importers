using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients
{
    public interface IMedicalHistoryDocument
    {
        IReadOnlyList<IMedicalHistoryMedicationDocument> Medications { get; }
        IReadOnlyList<IChronicConditionDocument> ChronicConditions { get; }
        IReadOnlyList<IMedicalHistoryAllergyDocument> Allergies { get; }
    }
}