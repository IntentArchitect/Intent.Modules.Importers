using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class MedicalRecord
    {
        private Guid? _id;

        public MedicalRecord()
        {
            RecordNumber = null!;
            RecordType = null!;
            ModifiedBy = null!;
            VisitInformation = null!;
            CreatedBy = null!;
            ClinicalData = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public Guid PatientId { get; set; }

        public string RecordNumber { get; set; }

        public string RecordType { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModified { get; set; }

        public string ModifiedBy { get; set; }

        public decimal Version { get; set; }

        public bool IsActive { get; set; }

        public VisitInformation VisitInformation { get; set; }

        public ICollection<MedicalRecordTreatmentPlan> TreatmentPlans { get; set; } = [];

        public ICollection<MedicalRecordMedication> Medications { get; set; } = [];

        public ICollection<DiagnosticTest> DiagnosticTests { get; set; } = [];

        public ICollection<MedicalRecordDiagnosis> Diagnoses { get; set; } = [];

        public CreatedBy CreatedBy { get; set; }

        public ClinicalDatum ClinicalData { get; set; }

        public ICollection<MedicalRecordAllergy> Allergies { get; set; } = [];
    }
}