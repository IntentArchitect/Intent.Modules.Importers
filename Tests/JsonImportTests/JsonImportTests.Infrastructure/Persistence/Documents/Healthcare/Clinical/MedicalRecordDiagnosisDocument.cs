using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class MedicalRecordDiagnosisDocument : IMedicalRecordDiagnosisDocument
    {
        public string Id { get; set; } = default!;
        public Guid DiagnosisId { get; set; }
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Severity { get; set; } = default!;
        public DateTime OnsetDate { get; set; }
        public string Status { get; set; } = default!;

        public MedicalRecordDiagnosis ToEntity(MedicalRecordDiagnosis? entity = default)
        {
            entity ??= new MedicalRecordDiagnosis();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.DiagnosisId = DiagnosisId;
            entity.Code = Code ?? throw new Exception($"{nameof(entity.Code)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Severity = Severity ?? throw new Exception($"{nameof(entity.Severity)} is null");
            entity.OnsetDate = OnsetDate;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");

            return entity;
        }

        public MedicalRecordDiagnosisDocument PopulateFromEntity(MedicalRecordDiagnosis entity)
        {
            Id = entity.Id;
            DiagnosisId = entity.DiagnosisId;
            Code = entity.Code;
            Description = entity.Description;
            Type = entity.Type;
            Severity = entity.Severity;
            OnsetDate = entity.OnsetDate;
            Status = entity.Status;

            return this;
        }

        public static MedicalRecordDiagnosisDocument? FromEntity(MedicalRecordDiagnosis? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalRecordDiagnosisDocument().PopulateFromEntity(entity);
        }
    }
}