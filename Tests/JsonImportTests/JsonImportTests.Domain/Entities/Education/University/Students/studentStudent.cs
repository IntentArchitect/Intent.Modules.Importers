using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class studentStudent
    {
        private Guid? _id;

        public studentStudent()
        {
            StudentId = null!;
            Technology = null!;
            PersonalInfo = null!;
            HealthAndServices = null!;
            FinancialInfo = null!;
            ContactInfo = null!;
            Activities = null!;
            AcademicInfo = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string StudentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool IsActive { get; set; }

        public bool PrivacyConsent { get; set; }

        public Technology Technology { get; set; }

        public StudentPersonalInfo PersonalInfo { get; set; }

        public HealthAndService HealthAndServices { get; set; }

        public StudentFinancialInfo FinancialInfo { get; set; }

        public ICollection<EnrollmentHistory> EnrollmentHistory { get; set; } = [];

        public StudentContactInfo ContactInfo { get; set; }

        public Activity Activities { get; set; }

        public AcademicInfo AcademicInfo { get; set; }
    }
}