using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Healthcare.Patients
{
    public class PrescriptionDto
    {
        public PrescriptionDto()
        {
            MedicationName = null!;
            Dosage = null!;
            Frequency = null!;
            Duration = null!;
        }

        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Duration { get; set; }

        public static PrescriptionDto Create(string medicationName, string dosage, string frequency, string duration)
        {
            return new PrescriptionDto
            {
                MedicationName = medicationName,
                Dosage = dosage,
                Frequency = frequency,
                Duration = duration
            };
        }
    }
}