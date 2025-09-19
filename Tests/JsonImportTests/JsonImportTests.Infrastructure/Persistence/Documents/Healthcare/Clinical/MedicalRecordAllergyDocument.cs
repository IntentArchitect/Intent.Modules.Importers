using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class MedicalRecordAllergyDocument : IMedicalRecordAllergyDocument
    {
        public string Id { get; set; } = default!;
        public Guid AllergyId { get; set; }
        public string Substance { get; set; } = default!;
        public string AllergyType { get; set; } = default!;
        public string Severity { get; set; } = default!;
        public string Reaction { get; set; } = default!;
        public DateTime OnsetDate { get; set; }
        public DateTime VerifiedDate { get; set; }

        public MedicalRecordAllergy ToEntity(MedicalRecordAllergy? entity = default)
        {
            entity ??= new MedicalRecordAllergy();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.AllergyId = AllergyId;
            entity.Substance = Substance ?? throw new Exception($"{nameof(entity.Substance)} is null");
            entity.AllergyType = AllergyType ?? throw new Exception($"{nameof(entity.AllergyType)} is null");
            entity.Severity = Severity ?? throw new Exception($"{nameof(entity.Severity)} is null");
            entity.Reaction = Reaction ?? throw new Exception($"{nameof(entity.Reaction)} is null");
            entity.OnsetDate = OnsetDate;
            entity.VerifiedDate = VerifiedDate;

            return entity;
        }

        public MedicalRecordAllergyDocument PopulateFromEntity(MedicalRecordAllergy entity)
        {
            Id = entity.Id;
            AllergyId = entity.AllergyId;
            Substance = entity.Substance;
            AllergyType = entity.AllergyType;
            Severity = entity.Severity;
            Reaction = entity.Reaction;
            OnsetDate = entity.OnsetDate;
            VerifiedDate = entity.VerifiedDate;

            return this;
        }

        public static MedicalRecordAllergyDocument? FromEntity(MedicalRecordAllergy? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalRecordAllergyDocument().PopulateFromEntity(entity);
        }
    }
}