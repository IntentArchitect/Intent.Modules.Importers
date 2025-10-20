using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.EdgeCases.ComplexStructures
{
    public class ArrayOfObjectDto
    {
        public ArrayOfObjectDto()
        {
            ItemName = null!;
        }

        public Guid ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemValue { get; set; }

        public static ArrayOfObjectDto Create(Guid itemId, string itemName, decimal itemValue)
        {
            return new ArrayOfObjectDto
            {
                ItemId = itemId,
                ItemName = itemName,
                ItemValue = itemValue
            };
        }
    }
}