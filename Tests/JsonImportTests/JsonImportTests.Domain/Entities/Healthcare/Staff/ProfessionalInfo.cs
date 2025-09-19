using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Staff
{
    public class ProfessionalInfo
    {
        public ProfessionalInfo()
        {
            Title = null!;
            Department = null!;
            Specialization = null!;
            LicenseNumber = null!;
        }

        public string Title { get; set; }

        public string Department { get; set; }

        public string Specialization { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime LicenseExpirationDate { get; set; }

        public decimal YearsOfExperience { get; set; }
    }
}