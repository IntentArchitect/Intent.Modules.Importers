using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Limit
    {
        public decimal APICallsPerDay { get; set; }

        public decimal StorageGB { get; set; }

        public decimal ConcurrentConnections { get; set; }
    }
}