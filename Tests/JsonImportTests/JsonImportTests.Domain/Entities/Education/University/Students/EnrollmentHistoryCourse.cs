using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class EnrollmentHistoryCourse
    {
        private string? _id;

        public EnrollmentHistoryCourse()
        {
            Id = null!;
            CourseCode = null!;
            Grade = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid CourseId { get; set; }

        public string CourseCode { get; set; }

        public decimal CreditHours { get; set; }

        public string Grade { get; set; }

        public decimal GradePoints { get; set; }

        public DateTime GradeDate { get; set; }
    }
}