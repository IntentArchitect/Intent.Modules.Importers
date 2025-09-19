using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Accreditation
    {
        public Accreditation()
        {
            AccreditingBody = null!;
            AccreditationLevel = null!;
            Status = null!;
        }

        public string AccreditingBody { get; set; }

        public string AccreditationLevel { get; set; }

        public DateTime LastReviewDate { get; set; }

        public DateTime NextReviewDate { get; set; }

        public string Status { get; set; }
    }
}