using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Setting
    {
        public Setting()
        {
            Theme = null!;
            Language = null!;
            Timezone = null!;
            Privacy = null!;
            Notifications = null!;
        }

        public string Theme { get; set; }

        public string Language { get; set; }

        public string Timezone { get; set; }

        public Privacy Privacy { get; set; }

        public Notification Notifications { get; set; }
    }
}