using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Patients
{
    public class ChronicCondition
    {
        private string? _id;

        public ChronicCondition()
        {
            Id = null!;
            Condition = null!;
            Status = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Condition { get; set; }

        public DateTime DiagnosedDate { get; set; }

        public string Status { get; set; }
    }
}