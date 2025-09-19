using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class MedicalHistory
    {
        public ICollection<MedicalHistoryMedication> Medications { get; set; } = [];

        public ICollection<ChronicCondition> ChronicConditions { get; set; } = [];

        public ICollection<MedicalHistoryAllergy> Allergies { get; set; } = [];
    }
}