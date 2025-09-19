using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class MedicalRecordTreatmentPlan
    {
        private string? _id;

        public MedicalRecordTreatmentPlan()
        {
            Id = null!;
            Description = null!;
            Status = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid PlanId { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Status { get; set; }

        public IList<string> Goals { get; set; } = [];

        public ICollection<Intervention> Interventions { get; set; } = [];
    }
}