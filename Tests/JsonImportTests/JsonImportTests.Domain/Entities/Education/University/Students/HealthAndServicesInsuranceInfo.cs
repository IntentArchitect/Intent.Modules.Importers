using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class HealthAndServicesInsuranceInfo
    {
        public HealthAndServicesInsuranceInfo()
        {
            InsuranceProvider = null!;
            PolicyNumber = null!;
            CoverageType = null!;
        }

        public bool HasInsurance { get; set; }

        public string InsuranceProvider { get; set; }

        public string PolicyNumber { get; set; }

        public string CoverageType { get; set; }
    }
}