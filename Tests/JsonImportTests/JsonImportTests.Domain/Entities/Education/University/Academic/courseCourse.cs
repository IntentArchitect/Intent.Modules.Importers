using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class courseCourse
    {
        private Guid? _id;

        public courseCourse()
        {
            CourseCode = null!;
            CourseName = null!;
            CourseTitle = null!;
            Description = null!;
            CourseLevel = null!;
            CreatedBy = null!;
            Department = null!;
            CourseContent = null!;
            Accreditation = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string CourseCode { get; set; }

        public string CourseName { get; set; }

        public string CourseTitle { get; set; }

        public string Description { get; set; }

        public decimal Credits { get; set; }

        public string CourseLevel { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime LastModified { get; set; }

        public string CreatedBy { get; set; }

        public bool IsActive { get; set; }

        public bool IsOfferedOnline { get; set; }

        public bool IsOfferedInPerson { get; set; }

        public ICollection<CoursePrerequisite> Prerequisites { get; set; } = [];

        public ICollection<Offering> Offerings { get; set; } = [];

        public ICollection<LearningOutcome> LearningOutcomes { get; set; } = [];

        public Department Department { get; set; }

        public CourseContent CourseContent { get; set; }

        public ICollection<Corequisite> Corequisites { get; set; } = [];

        public Accreditation Accreditation { get; set; }
    }
}