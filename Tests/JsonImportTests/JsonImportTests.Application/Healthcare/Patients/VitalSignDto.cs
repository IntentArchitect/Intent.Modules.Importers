using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Healthcare.Patients
{
    public class VitalSignDto
    {
        public VitalSignDto()
        {
            BloodPressure = null!;
        }

        public decimal Temperature { get; set; }
        public string BloodPressure { get; set; }
        public decimal HeartRate { get; set; }
        public decimal RespiratoryRate { get; set; }

        public static VitalSignDto Create(
            decimal temperature,
            string bloodPressure,
            decimal heartRate,
            decimal respiratoryRate)
        {
            return new VitalSignDto
            {
                Temperature = temperature,
                BloodPressure = bloodPressure,
                HeartRate = heartRate,
                RespiratoryRate = respiratoryRate
            };
        }
    }
}