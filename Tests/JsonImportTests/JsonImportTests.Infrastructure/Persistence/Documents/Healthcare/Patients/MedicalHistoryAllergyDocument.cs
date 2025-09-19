using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class MedicalHistoryAllergyDocument : IMedicalHistoryAllergyDocument
    {
        public string Id { get; set; } = default!;
        public string Substance { get; set; } = default!;
        public string Severity { get; set; } = default!;
        public string Reaction { get; set; } = default!;

        public MedicalHistoryAllergy ToEntity(MedicalHistoryAllergy? entity = default)
        {
            entity ??= new MedicalHistoryAllergy();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Substance = Substance ?? throw new Exception($"{nameof(entity.Substance)} is null");
            entity.Severity = Severity ?? throw new Exception($"{nameof(entity.Severity)} is null");
            entity.Reaction = Reaction ?? throw new Exception($"{nameof(entity.Reaction)} is null");

            return entity;
        }

        public MedicalHistoryAllergyDocument PopulateFromEntity(MedicalHistoryAllergy entity)
        {
            Id = entity.Id;
            Substance = entity.Substance;
            Severity = entity.Severity;
            Reaction = entity.Reaction;

            return this;
        }

        public static MedicalHistoryAllergyDocument? FromEntity(MedicalHistoryAllergy? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalHistoryAllergyDocument().PopulateFromEntity(entity);
        }
    }
}