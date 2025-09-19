using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class PrerequisiteCheck
    {
        private string? _id;

        public PrerequisiteCheck()
        {
            Id = null!;
            RequiredCourse = null!;
            MinimumGrade = null!;
            StudentGrade = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid RequiredCourseId { get; set; }

        public string RequiredCourse { get; set; }

        public string MinimumGrade { get; set; }

        public string StudentGrade { get; set; }

        public bool IsMet { get; set; }

        public DateTime CheckedDate { get; set; }
    }
}