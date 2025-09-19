using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class SpecificProperty
    {
        public SpecificProperty()
        {
            VehicleType = null!;
            FuelType = null!;
            Transmission = null!;
            Safety = null!;
            Engine = null!;
        }

        public string VehicleType { get; set; }

        public decimal Doors { get; set; }

        public string FuelType { get; set; }

        public string Transmission { get; set; }

        public IList<string> Features { get; set; } = [];

        public Safety Safety { get; set; }

        public Engine Engine { get; set; }
    }
}