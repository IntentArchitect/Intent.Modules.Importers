using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class MixedTypeArray
    {
        private string? _id;

        public MixedTypeArray()
        {
            Id = null!;
            Type = null!;
            Value = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Type { get; set; }

        public string Value { get; set; }

        public bool IsValid { get; set; }
    }
}