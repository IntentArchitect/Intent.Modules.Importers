using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class MedicalRecordMedication
    {
        private string? _id;

        public MedicalRecordMedication()
        {
            Id = null!;
            Name = null!;
            GenericName = null!;
            Dosage = null!;
            Route = null!;
            Frequency = null!;
            PrescribingPhysician = null!;
            Instructions = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid MedicationId { get; set; }

        public string Name { get; set; }

        public string GenericName { get; set; }

        public string Dosage { get; set; }

        public string Route { get; set; }

        public string Frequency { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string PrescribingPhysician { get; set; }

        public string Instructions { get; set; }
    }
}