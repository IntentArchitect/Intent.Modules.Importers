using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class MetadataMap
    {
        public MetadataMap()
        {
            Key1 = null!;
            Key4 = null!;
            Key5 = null!;
        }

        public string Key1 { get; set; }

        public decimal Key2 { get; set; }

        public bool Key3 { get; set; }

        public object Key4 { get; set; }

        public Key5 Key5 { get; set; }
    }
}