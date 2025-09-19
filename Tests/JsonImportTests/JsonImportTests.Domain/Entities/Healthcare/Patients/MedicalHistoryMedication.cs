using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class MedicalHistoryMedication
    {
        private string? _id;

        public MedicalHistoryMedication()
        {
            Id = null!;
            Name = null!;
            Dosage = null!;
            Frequency = null!;
            PrescribingPhysician = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Dosage { get; set; }

        public string Frequency { get; set; }

        public DateTime PrescribedDate { get; set; }

        public string PrescribingPhysician { get; set; }
    }
}