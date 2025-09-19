using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class MedicalRecordDocument : IMedicalRecordDocument, ICosmosDBDocument<MedicalRecord, MedicalRecordDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public Guid PatientId { get; set; }
        public string RecordNumber { get; set; } = default!;
        public string RecordType { get; set; } = default!;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string ModifiedBy { get; set; } = default!;
        public decimal Version { get; set; }
        public bool IsActive { get; set; }
        public VisitInformationDocument VisitInformation { get; set; } = default!;
        IVisitInformationDocument IMedicalRecordDocument.VisitInformation => VisitInformation;
        public List<MedicalRecordTreatmentPlanDocument> TreatmentPlans { get; set; } = default!;
        IReadOnlyList<IMedicalRecordTreatmentPlanDocument> IMedicalRecordDocument.TreatmentPlans => TreatmentPlans;
        public List<MedicalRecordMedicationDocument> Medications { get; set; } = default!;
        IReadOnlyList<IMedicalRecordMedicationDocument> IMedicalRecordDocument.Medications => Medications;
        public List<DiagnosticTestDocument> DiagnosticTests { get; set; } = default!;
        IReadOnlyList<IDiagnosticTestDocument> IMedicalRecordDocument.DiagnosticTests => DiagnosticTests;
        public List<MedicalRecordDiagnosisDocument> Diagnoses { get; set; } = default!;
        IReadOnlyList<IMedicalRecordDiagnosisDocument> IMedicalRecordDocument.Diagnoses => Diagnoses;
        public CreatedByDocument CreatedBy { get; set; } = default!;
        ICreatedByDocument IMedicalRecordDocument.CreatedBy => CreatedBy;
        public ClinicalDatumDocument ClinicalData { get; set; } = default!;
        IClinicalDatumDocument IMedicalRecordDocument.ClinicalData => ClinicalData;
        public List<MedicalRecordAllergyDocument> Allergies { get; set; } = default!;
        IReadOnlyList<IMedicalRecordAllergyDocument> IMedicalRecordDocument.Allergies => Allergies;

        public MedicalRecord ToEntity(MedicalRecord? entity = default)
        {
            entity ??= new MedicalRecord();

            entity.Id = Guid.Parse(Id);
            entity.PatientId = PatientId;
            entity.RecordNumber = RecordNumber ?? throw new Exception($"{nameof(entity.RecordNumber)} is null");
            entity.RecordType = RecordType ?? throw new Exception($"{nameof(entity.RecordType)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastModified = LastModified;
            entity.ModifiedBy = ModifiedBy ?? throw new Exception($"{nameof(entity.ModifiedBy)} is null");
            entity.Version = Version;
            entity.IsActive = IsActive;
            entity.VisitInformation = VisitInformation.ToEntity() ?? throw new Exception($"{nameof(entity.VisitInformation)} is null");
            entity.TreatmentPlans = TreatmentPlans.Select(x => x.ToEntity()).ToList();
            entity.Medications = Medications.Select(x => x.ToEntity()).ToList();
            entity.DiagnosticTests = DiagnosticTests.Select(x => x.ToEntity()).ToList();
            entity.Diagnoses = Diagnoses.Select(x => x.ToEntity()).ToList();
            entity.CreatedBy = CreatedBy.ToEntity() ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.ClinicalData = ClinicalData.ToEntity() ?? throw new Exception($"{nameof(entity.ClinicalData)} is null");
            entity.Allergies = Allergies.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public MedicalRecordDocument PopulateFromEntity(MedicalRecord entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            PatientId = entity.PatientId;
            RecordNumber = entity.RecordNumber;
            RecordType = entity.RecordType;
            CreatedDate = entity.CreatedDate;
            LastModified = entity.LastModified;
            ModifiedBy = entity.ModifiedBy;
            Version = entity.Version;
            IsActive = entity.IsActive;
            VisitInformation = VisitInformationDocument.FromEntity(entity.VisitInformation)!;
            TreatmentPlans = entity.TreatmentPlans.Select(x => MedicalRecordTreatmentPlanDocument.FromEntity(x)!).ToList();
            Medications = entity.Medications.Select(x => MedicalRecordMedicationDocument.FromEntity(x)!).ToList();
            DiagnosticTests = entity.DiagnosticTests.Select(x => DiagnosticTestDocument.FromEntity(x)!).ToList();
            Diagnoses = entity.Diagnoses.Select(x => MedicalRecordDiagnosisDocument.FromEntity(x)!).ToList();
            CreatedBy = CreatedByDocument.FromEntity(entity.CreatedBy)!;
            ClinicalData = ClinicalDatumDocument.FromEntity(entity.ClinicalData)!;
            Allergies = entity.Allergies.Select(x => MedicalRecordAllergyDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static MedicalRecordDocument? FromEntity(MedicalRecord? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new MedicalRecordDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}