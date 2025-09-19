using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class StudentContactInfoAddress
    {
        public StudentContactInfoAddress()
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

        public bool IsInternational { get; set; }
    }
}