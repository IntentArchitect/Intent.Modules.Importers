using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class MedicalRecordMedicationDocument : IMedicalRecordMedicationDocument
    {
        public string Id { get; set; } = default!;
        public Guid MedicationId { get; set; }
        public string Name { get; set; } = default!;
        public string GenericName { get; set; } = default!;
        public string Dosage { get; set; } = default!;
        public string Route { get; set; } = default!;
        public string Frequency { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PrescribingPhysician { get; set; } = default!;
        public string Instructions { get; set; } = default!;

        public MedicalRecordMedication ToEntity(MedicalRecordMedication? entity = default)
        {
            entity ??= new MedicalRecordMedication();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.MedicationId = MedicationId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.GenericName = GenericName ?? throw new Exception($"{nameof(entity.GenericName)} is null");
            entity.Dosage = Dosage ?? throw new Exception($"{nameof(entity.Dosage)} is null");
            entity.Route = Route ?? throw new Exception($"{nameof(entity.Route)} is null");
            entity.Frequency = Frequency ?? throw new Exception($"{nameof(entity.Frequency)} is null");
            entity.StartDate = StartDate;
            entity.EndDate = EndDate;
            entity.PrescribingPhysician = PrescribingPhysician ?? throw new Exception($"{nameof(entity.PrescribingPhysician)} is null");
            entity.Instructions = Instructions ?? throw new Exception($"{nameof(entity.Instructions)} is null");

            return entity;
        }

        public MedicalRecordMedicationDocument PopulateFromEntity(MedicalRecordMedication entity)
        {
            Id = entity.Id;
            MedicationId = entity.MedicationId;
            Name = entity.Name;
            GenericName = entity.GenericName;
            Dosage = entity.Dosage;
            Route = entity.Route;
            Frequency = entity.Frequency;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
            PrescribingPhysician = entity.PrescribingPhysician;
            Instructions = entity.Instructions;

            return this;
        }

        public static MedicalRecordMedicationDocument? FromEntity(MedicalRecordMedication? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalRecordMedicationDocument().PopulateFromEntity(entity);
        }
    }
}