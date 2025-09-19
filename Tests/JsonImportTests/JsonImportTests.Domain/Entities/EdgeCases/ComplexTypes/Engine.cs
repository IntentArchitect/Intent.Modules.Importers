using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Engine
    {
        public Engine()
        {
            Type = null!;
        }

        public string Type { get; set; }

        public decimal Displacement { get; set; }

        public decimal Horsepower { get; set; }

        public bool Turbo { get; set; }
    }
}