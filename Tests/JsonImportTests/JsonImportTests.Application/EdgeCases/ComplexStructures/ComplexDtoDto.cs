using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.EdgeCases.ComplexStructures
{
    public class ComplexDtoDto
    {
        public ComplexDtoDto()
        {
            Name = null!;
            PrimitiveString = null!;
            ArrayOfStrings = null!;
            ArrayOfNumbers = null!;
            ArrayOfBooleans = null!;
            NestedObject = null!;
            ArrayOfObjects = null!;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PrimitiveString { get; set; }
        public decimal PrimitiveNumber { get; set; }
        public decimal PrimitiveDecimal { get; set; }
        public bool PrimitiveBoolean { get; set; }
        public Guid PrimitiveGuid { get; set; }
        public DateTime PrimitiveDateTime { get; set; }
        public List<string> ArrayOfStrings { get; set; }
        public List<decimal> ArrayOfNumbers { get; set; }
        public List<bool> ArrayOfBooleans { get; set; }
        public NestedObjectDto NestedObject { get; set; }
        public List<ArrayOfObjectDto> ArrayOfObjects { get; set; }

        public static ComplexDtoDto Create(
            Guid id,
            string name,
            string primitiveString,
            decimal primitiveNumber,
            decimal primitiveDecimal,
            bool primitiveBoolean,
            Guid primitiveGuid,
            DateTime primitiveDateTime,
            List<string> arrayOfStrings,
            List<decimal> arrayOfNumbers,
            List<bool> arrayOfBooleans,
            NestedObjectDto nestedObject,
            List<ArrayOfObjectDto> arrayOfObjects)
        {
            return new ComplexDtoDto
            {
                Id = id,
                Name = name,
                PrimitiveString = primitiveString,
                PrimitiveNumber = primitiveNumber,
                PrimitiveDecimal = primitiveDecimal,
                PrimitiveBoolean = primitiveBoolean,
                PrimitiveGuid = primitiveGuid,
                PrimitiveDateTime = primitiveDateTime,
                ArrayOfStrings = arrayOfStrings,
                ArrayOfNumbers = arrayOfNumbers,
                ArrayOfBooleans = arrayOfBooleans,
                NestedObject = nestedObject,
                ArrayOfObjects = arrayOfObjects
            };
        }
    }
}