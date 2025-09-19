using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class PatientPersonalInfo
    {
        public PatientPersonalInfo()
        {
            FirstName = null!;
            LastName = null!;
            Gender = null!;
            SocialSecurityNumber = null!;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string SocialSecurityNumber { get; set; }
    }
}