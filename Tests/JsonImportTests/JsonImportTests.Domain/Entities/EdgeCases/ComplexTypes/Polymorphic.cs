using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Polymorphic
    {
        private string? _id;

        public Polymorphic()
        {
            Id = null!;
            Type = null!;
            Color = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Type { get; set; }

        public decimal Radius { get; set; }

        public string Color { get; set; }
    }
}