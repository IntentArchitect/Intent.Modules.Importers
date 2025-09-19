using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class Intervention
    {
        private string? _id;

        public Intervention()
        {
            Id = null!;
            Type = null!;
            Description = null!;
            Frequency = null!;
            Duration = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Type { get; set; }

        public string Description { get; set; }

        public string Frequency { get; set; }

        public string Duration { get; set; }
    }
}