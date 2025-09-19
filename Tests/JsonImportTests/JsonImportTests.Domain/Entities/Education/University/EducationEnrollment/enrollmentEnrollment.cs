using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class enrollmentEnrollment
    {
        private Guid? _id;

        public enrollmentEnrollment()
        {
            EnrollmentStatus = null!;
            EnrollmentType = null!;
            ModifiedBy = null!;
            WithdrawalInfo = null!;
            Student = null!;
            SpecialCircumstances = null!;
            Semester = null!;
            Prerequisites = null!;
            Participation = null!;
            Instructor = null!;
            GradingInfo = null!;
            FinancialInfo = null!;
            Course = null!;
            Attendance = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public Guid StudentId { get; set; }

        public Guid CourseOfferingId { get; set; }

        public Guid SemesterId { get; set; }

        public DateTime EnrollmentDate { get; set; }

        public string EnrollmentStatus { get; set; }

        public string EnrollmentType { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModified { get; set; }

        public string ModifiedBy { get; set; }

        public decimal Version { get; set; }

        public bool IsActive { get; set; }

        public WithdrawalInfo WithdrawalInfo { get; set; }

        public EnrollmentStudent Student { get; set; }

        public SpecialCircumstance SpecialCircumstances { get; set; }

        public Semester Semester { get; set; }

        public EnrollmentPrerequisite Prerequisites { get; set; }

        public Participation Participation { get; set; }

        public EnrollmentInstructor Instructor { get; set; }

        public GradingInfo GradingInfo { get; set; }

        public EnrollmentFinancialInfo FinancialInfo { get; set; }

        public EnrollmentCourse Course { get; set; }

        public Attendance Attendance { get; set; }

        public ICollection<Assessment> Assessments { get; set; } = [];
    }
}