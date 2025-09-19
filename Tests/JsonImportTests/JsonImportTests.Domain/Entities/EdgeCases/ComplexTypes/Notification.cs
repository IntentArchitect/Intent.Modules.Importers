using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Notification
    {
        public bool Email { get; set; }

        public bool SMS { get; set; }

        public bool Push { get; set; }

        public bool InApp { get; set; }
    }
}