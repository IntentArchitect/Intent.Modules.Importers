using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.EdgeCases.ComplexStructures
{
    public class DeeplyNestedDto
    {
        public DeeplyNestedDto()
        {
            Value = null!;
        }

        public decimal Level { get; set; }
        public string Value { get; set; }

        public static DeeplyNestedDto Create(decimal level, string value)
        {
            return new DeeplyNestedDto
            {
                Level = level,
                Value = value
            };
        }
    }
}