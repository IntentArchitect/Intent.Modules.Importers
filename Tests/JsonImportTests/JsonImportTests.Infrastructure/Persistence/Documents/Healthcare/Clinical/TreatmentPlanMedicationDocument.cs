using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class TreatmentPlanMedicationDocument : ITreatmentPlanMedicationDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Dosage { get; set; } = default!;
        public string Frequency { get; set; } = default!;
        public string Duration { get; set; } = default!;

        public TreatmentPlanMedication ToEntity(TreatmentPlanMedication? entity = default)
        {
            entity ??= new TreatmentPlanMedication();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Dosage = Dosage ?? throw new Exception($"{nameof(entity.Dosage)} is null");
            entity.Frequency = Frequency ?? throw new Exception($"{nameof(entity.Frequency)} is null");
            entity.Duration = Duration ?? throw new Exception($"{nameof(entity.Duration)} is null");

            return entity;
        }

        public TreatmentPlanMedicationDocument PopulateFromEntity(TreatmentPlanMedication entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Dosage = entity.Dosage;
            Frequency = entity.Frequency;
            Duration = entity.Duration;

            return this;
        }

        public static TreatmentPlanMedicationDocument? FromEntity(TreatmentPlanMedication? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TreatmentPlanMedicationDocument().PopulateFromEntity(entity);
        }
    }
}