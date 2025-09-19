using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class VersionHistory
    {
        private string? _id;

        public VersionHistory()
        {
            Id = null!;
            Version = null!;
            Author = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Version { get; set; }

        public IList<string> Changes { get; set; } = [];

        public DateTime Date { get; set; }

        public string Author { get; set; }

        public bool Breaking { get; set; }
    }
}