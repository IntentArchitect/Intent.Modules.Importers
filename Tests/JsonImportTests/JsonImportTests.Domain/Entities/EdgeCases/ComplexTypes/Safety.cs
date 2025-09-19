using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Safety
    {
        public decimal AirbagsCount { get; set; }

        public bool HasABS { get; set; }

        public bool HasESC { get; set; }

        public decimal CrashRating { get; set; }

        public IList<string> OptionalSafetyFeatures { get; set; } = [];
    }
}