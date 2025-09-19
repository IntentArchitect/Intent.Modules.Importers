using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Configuration
    {
        public Configuration()
        {
            Settings = null!;
        }

        public Setting Settings { get; set; }

        public ICollection<Preference> Preferences { get; set; } = [];
    }
}