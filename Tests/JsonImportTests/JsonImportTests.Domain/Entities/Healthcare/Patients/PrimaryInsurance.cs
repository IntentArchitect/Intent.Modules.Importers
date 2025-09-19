using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class PrimaryInsurance
    {
        public PrimaryInsurance()
        {
            ProviderName = null!;
            PolicyNumber = null!;
            GroupNumber = null!;
        }

        public string ProviderName { get; set; }

        public string PolicyNumber { get; set; }

        public string GroupNumber { get; set; }

        public DateTime EffectiveDate { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}