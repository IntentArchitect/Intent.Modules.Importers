using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class StudentContactInfo
    {
        public StudentContactInfo()
        {
            Email = null!;
            AlternateEmail = null!;
            Phone = null!;
            EmergencyContact = null!;
            Address = null!;
        }

        public string Email { get; set; }

        public string AlternateEmail { get; set; }

        public string Phone { get; set; }

        public ContactInfoEmergencyContact EmergencyContact { get; set; }

        public StudentContactInfoAddress Address { get; set; }
    }
}