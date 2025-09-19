using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class VisitDetailsVitalSign
    {
        public decimal Temperature { get; set; }

        public decimal BloodPressureSystolic { get; set; }

        public decimal BloodPressureDiastolic { get; set; }

        public decimal HeartRate { get; set; }

        public decimal RespiratoryRate { get; set; }

        public decimal OxygenSaturation { get; set; }

        public decimal Weight { get; set; }

        public decimal Height { get; set; }
    }
}