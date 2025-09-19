using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class PatientContactInfo
    {
        public PatientContactInfo()
        {
            PrimaryPhone = null!;
            SecondaryPhone = null!;
            Email = null!;
            Address = null!;
        }

        public string PrimaryPhone { get; set; }

        public string SecondaryPhone { get; set; }

        public string Email { get; set; }

        public PatientContactInfoAddress Address { get; set; }
    }
}