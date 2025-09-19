using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Staff
{
    public class PractitionerContactInfo
    {
        public PractitionerContactInfo()
        {
            Phone = null!;
            Email = null!;
            Address = null!;
        }

        public string Phone { get; set; }

        public string Email { get; set; }

        public PractitionerContactInfoAddress Address { get; set; }
    }
}