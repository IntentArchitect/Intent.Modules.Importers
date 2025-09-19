using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class PatientInsuranceInfo
    {
        public PatientInsuranceInfo()
        {
            SecondaryInsurance = null!;
            PrimaryInsurance = null!;
        }

        public SecondaryInsurance SecondaryInsurance { get; set; }

        public PrimaryInsurance PrimaryInsurance { get; set; }
    }
}