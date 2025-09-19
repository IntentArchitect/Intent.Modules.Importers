using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class MedicalHistoryMedicationDocument : IMedicalHistoryMedicationDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Dosage { get; set; } = default!;
        public string Frequency { get; set; } = default!;
        public DateTime PrescribedDate { get; set; }
        public string PrescribingPhysician { get; set; } = default!;

        public MedicalHistoryMedication ToEntity(MedicalHistoryMedication? entity = default)
        {
            entity ??= new MedicalHistoryMedication();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Dosage = Dosage ?? throw new Exception($"{nameof(entity.Dosage)} is null");
            entity.Frequency = Frequency ?? throw new Exception($"{nameof(entity.Frequency)} is null");
            entity.PrescribedDate = PrescribedDate;
            entity.PrescribingPhysician = PrescribingPhysician ?? throw new Exception($"{nameof(entity.PrescribingPhysician)} is null");

            return entity;
        }

        public MedicalHistoryMedicationDocument PopulateFromEntity(MedicalHistoryMedication entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Dosage = entity.Dosage;
            Frequency = entity.Frequency;
            PrescribedDate = entity.PrescribedDate;
            PrescribingPhysician = entity.PrescribingPhysician;

            return this;
        }

        public static MedicalHistoryMedicationDocument? FromEntity(MedicalHistoryMedication? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalHistoryMedicationDocument().PopulateFromEntity(entity);
        }
    }
}