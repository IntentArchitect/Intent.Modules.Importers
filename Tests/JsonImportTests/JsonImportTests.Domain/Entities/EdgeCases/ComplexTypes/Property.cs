using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Property
    {
        public Property()
        {
            Make = null!;
            Model = null!;
            VIN = null!;
        }

        public string Make { get; set; }

        public string Model { get; set; }

        public decimal Year { get; set; }

        public string VIN { get; set; }
    }
}