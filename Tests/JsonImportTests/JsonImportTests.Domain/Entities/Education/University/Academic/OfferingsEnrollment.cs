using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class OfferingsEnrollment
    {
        public decimal MaxStudents { get; set; }

        public decimal EnrolledStudents { get; set; }

        public decimal WaitlistStudents { get; set; }

        public decimal MinEnrollment { get; set; }
    }
}