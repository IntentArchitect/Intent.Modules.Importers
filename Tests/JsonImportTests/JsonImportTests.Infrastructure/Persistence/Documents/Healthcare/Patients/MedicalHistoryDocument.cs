using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class MedicalHistoryDocument : IMedicalHistoryDocument
    {
        public List<MedicalHistoryMedicationDocument> Medications { get; set; } = default!;
        IReadOnlyList<IMedicalHistoryMedicationDocument> IMedicalHistoryDocument.Medications => Medications;
        public List<ChronicConditionDocument> ChronicConditions { get; set; } = default!;
        IReadOnlyList<IChronicConditionDocument> IMedicalHistoryDocument.ChronicConditions => ChronicConditions;
        public List<MedicalHistoryAllergyDocument> Allergies { get; set; } = default!;
        IReadOnlyList<IMedicalHistoryAllergyDocument> IMedicalHistoryDocument.Allergies => Allergies;

        public MedicalHistory ToEntity(MedicalHistory? entity = default)
        {
            entity ??= new MedicalHistory();
            entity.Medications = Medications.Select(x => x.ToEntity()).ToList();
            entity.ChronicConditions = ChronicConditions.Select(x => x.ToEntity()).ToList();
            entity.Allergies = Allergies.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public MedicalHistoryDocument PopulateFromEntity(MedicalHistory entity)
        {
            Medications = entity.Medications.Select(x => MedicalHistoryMedicationDocument.FromEntity(x)!).ToList();
            ChronicConditions = entity.ChronicConditions.Select(x => ChronicConditionDocument.FromEntity(x)!).ToList();
            Allergies = entity.Allergies.Select(x => MedicalHistoryAllergyDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static MedicalHistoryDocument? FromEntity(MedicalHistory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalHistoryDocument().PopulateFromEntity(entity);
        }
    }
}