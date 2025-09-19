using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class SystemicFinding
    {
        public SystemicFinding()
        {
            Head = null!;
            Neck = null!;
            Chest = null!;
            Abdomen = null!;
            Extremities = null!;
        }

        public string Head { get; set; }

        public string Neck { get; set; }

        public string Chest { get; set; }

        public string Abdomen { get; set; }

        public string Extremities { get; set; }
    }
}