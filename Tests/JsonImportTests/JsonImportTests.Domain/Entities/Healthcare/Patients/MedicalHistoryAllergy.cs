using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class MedicalHistoryAllergy
    {
        private string? _id;

        public MedicalHistoryAllergy()
        {
            Id = null!;
            Substance = null!;
            Severity = null!;
            Reaction = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Substance { get; set; }

        public string Severity { get; set; }

        public string Reaction { get; set; }
    }
}