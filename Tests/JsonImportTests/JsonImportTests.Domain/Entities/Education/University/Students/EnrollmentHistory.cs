using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class EnrollmentHistory
    {
        private string? _id;

        public EnrollmentHistory()
        {
            Id = null!;
            Semester = null!;
            EnrollmentStatus = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid SemesterId { get; set; }

        public string Semester { get; set; }

        public decimal Year { get; set; }

        public string EnrollmentStatus { get; set; }

        public decimal CreditHours { get; set; }

        public decimal SemesterGPA { get; set; }

        public bool DeansList { get; set; }

        public bool Probation { get; set; }

        public ICollection<EnrollmentHistoryCourse> Courses { get; set; } = [];
    }
}