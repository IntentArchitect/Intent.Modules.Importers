using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class OptionalNestedObject
    {
        public OptionalNestedObject()
        {
            RequiredField = null!;
            OptionalField = null!;
            ConditionalData = null!;
        }

        public string RequiredField { get; set; }

        public object OptionalField { get; set; }

        public ConditionalDatum ConditionalData { get; set; }
    }
}