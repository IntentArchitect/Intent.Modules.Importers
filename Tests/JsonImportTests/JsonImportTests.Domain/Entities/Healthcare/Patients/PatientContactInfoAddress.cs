using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class PatientContactInfoAddress
    {
        public PatientContactInfoAddress()
        {
            Street = null!;
            City = null!;
            State = null!;
            ZipCode = null!;
            Country = null!;
        }

        public string Street { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Country { get; set; }
    }
}