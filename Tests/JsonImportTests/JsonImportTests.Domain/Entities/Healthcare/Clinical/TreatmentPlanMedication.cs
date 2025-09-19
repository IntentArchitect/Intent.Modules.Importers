using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class TreatmentPlanMedication
    {
        private string? _id;

        public TreatmentPlanMedication()
        {
            Id = null!;
            Name = null!;
            Dosage = null!;
            Frequency = null!;
            Duration = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Name { get; set; }

        public string Dosage { get; set; }

        public string Frequency { get; set; }

        public string Duration { get; set; }
    }
}