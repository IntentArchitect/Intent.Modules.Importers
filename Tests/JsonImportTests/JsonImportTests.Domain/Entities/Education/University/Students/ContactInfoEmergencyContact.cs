using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class ContactInfoEmergencyContact
    {
        public ContactInfoEmergencyContact()
        {
            Name = null!;
            Relationship = null!;
            Phone = null!;
            Email = null!;
        }

        public string Name { get; set; }

        public string Relationship { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}