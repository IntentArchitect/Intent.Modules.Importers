using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Preference
    {
        private string? _id;

        public Preference()
        {
            Id = null!;
            Category = null!;
            Key = null!;
            Value = null!;
            Type = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Category { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }
    }
}