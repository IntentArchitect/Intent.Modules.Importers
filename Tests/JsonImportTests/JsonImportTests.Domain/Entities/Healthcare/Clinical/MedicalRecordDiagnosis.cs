using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class MedicalRecordDiagnosis
    {
        private string? _id;

        public MedicalRecordDiagnosis()
        {
            Id = null!;
            Code = null!;
            Description = null!;
            Type = null!;
            Severity = null!;
            Status = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid DiagnosisId { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }

        public string Severity { get; set; }

        public DateTime OnsetDate { get; set; }

        public string Status { get; set; }
    }
}