using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Timestamp
    {
        public Timestamp()
        {
            LastAccessedAt = null!;
            CompletedAt = null!;
            DeletedAt = null!;
        }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public object LastAccessedAt { get; set; }

        public DateTime ScheduledAt { get; set; }

        public object CompletedAt { get; set; }

        public object DeletedAt { get; set; }
    }
}