using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class MedicalRecordAllergy
    {
        private string? _id;

        public MedicalRecordAllergy()
        {
            Id = null!;
            Substance = null!;
            AllergyType = null!;
            Severity = null!;
            Reaction = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid AllergyId { get; set; }

        public string Substance { get; set; }

        public string AllergyType { get; set; }

        public string Severity { get; set; }

        public string Reaction { get; set; }

        public DateTime OnsetDate { get; set; }

        public DateTime VerifiedDate { get; set; }
    }
}