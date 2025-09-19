using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class Offering
    {
        private Guid? _offeringId;

        public Offering()
        {
            Semester = null!;
            Section = null!;
            Schedule = null!;
            Instructor = null!;
            GradingPolicy = null!;
            Enrollment = null!;
        }

        public Guid OfferingId
        {
            get => _offeringId ??= Guid.NewGuid();
            set => _offeringId = value;
        }

        public string Semester { get; set; }

        public decimal Year { get; set; }

        public string Section { get; set; }

        public Schedule Schedule { get; set; }

        public OfferingsInstructor Instructor { get; set; }

        public GradingPolicy GradingPolicy { get; set; }

        public OfferingsEnrollment Enrollment { get; set; }
    }
}