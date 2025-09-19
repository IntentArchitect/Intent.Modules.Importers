using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class ReviewOfSystem
    {
        public ReviewOfSystem()
        {
            Constitutional = null!;
            Cardiovascular = null!;
            Respiratory = null!;
            Gastrointestinal = null!;
            Neurological = null!;
            Musculoskeletal = null!;
        }

        public string Constitutional { get; set; }

        public string Cardiovascular { get; set; }

        public string Respiratory { get; set; }

        public string Gastrointestinal { get; set; }

        public string Neurological { get; set; }

        public string Musculoskeletal { get; set; }
    }
}