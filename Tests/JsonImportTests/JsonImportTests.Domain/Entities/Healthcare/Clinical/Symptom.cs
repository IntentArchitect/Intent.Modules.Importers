using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class Symptom
    {
        private string? _id;

        public Symptom()
        {
            Id = null!;
            Description = null!;
            Severity = null!;
            Duration = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Description { get; set; }

        public string Severity { get; set; }

        public string Duration { get; set; }

        public DateTime OnsetDate { get; set; }
    }
}