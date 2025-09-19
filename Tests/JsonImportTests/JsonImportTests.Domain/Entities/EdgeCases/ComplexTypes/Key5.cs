using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Key5
    {
        public Key5()
        {
            Nested = null!;
        }

        public string Nested { get; set; }

        public IList<string> With { get; set; } = [];
    }
}