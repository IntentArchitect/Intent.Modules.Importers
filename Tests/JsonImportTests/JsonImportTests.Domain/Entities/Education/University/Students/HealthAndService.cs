using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class HealthAndService
    {
        public HealthAndService()
        {
            InsuranceInfo = null!;
            CounselingServices = null!;
        }

        public HealthAndServicesInsuranceInfo InsuranceInfo { get; set; }

        public ICollection<Disability> Disabilities { get; set; } = [];

        public CounselingService CounselingServices { get; set; }
    }
}