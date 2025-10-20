using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.EdgeCases.ComplexStructures
{
    public class NestedObjectDto
    {
        public NestedObjectDto()
        {
            NestedName = null!;
            DeeplyNested = null!;
        }

        public Guid NestedId { get; set; }
        public string NestedName { get; set; }
        public DeeplyNestedDto DeeplyNested { get; set; }

        public static NestedObjectDto Create(Guid nestedId, string nestedName, DeeplyNestedDto deeplyNested)
        {
            return new NestedObjectDto
            {
                NestedId = nestedId,
                NestedName = nestedName,
                DeeplyNested = deeplyNested
            };
        }
    }
}