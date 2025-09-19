using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class ConditionalDatum
    {
        public ConditionalDatum()
        {
            WhenPresent = null!;
            MaybeNull = null!;
        }

        public string WhenPresent { get; set; }

        public object MaybeNull { get; set; }

        public IList<string> OptionalArray { get; set; } = [];
    }
}