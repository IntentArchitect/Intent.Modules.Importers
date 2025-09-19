using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes
{
    public class Privacy
    {
        public Privacy()
        {
            ProfileVisibility = null!;
            ShowPhone = null!;
        }

        public string ProfileVisibility { get; set; }

        public bool ShowEmail { get; set; }

        public object ShowPhone { get; set; }

        public bool AllowSearch { get; set; }
    }
}