using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class PhysicalExaminationVitalSign
    {
        public PhysicalExaminationVitalSign()
        {
            BloodPressure = null!;
        }

        public decimal Temperature { get; set; }

        public string BloodPressure { get; set; }

        public decimal HeartRate { get; set; }

        public decimal RespiratoryRate { get; set; }

        public decimal OxygenSaturation { get; set; }
    }
}