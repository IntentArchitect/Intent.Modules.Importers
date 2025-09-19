using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class AcademicInfo
    {
        public AcademicInfo()
        {
            StudentType = null!;
            AcademicStatus = null!;
            ClassLevel = null!;
            GPA = null!;
            AcademicAdvisor = null!;
        }

        public DateTime AdmissionDate { get; set; }

        public string StudentType { get; set; }

        public string AcademicStatus { get; set; }

        public string ClassLevel { get; set; }

        public DateTime ExpectedGraduationDate { get; set; }

        public ICollection<MajorProgram> MajorPrograms { get; set; } = [];

        public GPA GPA { get; set; }

        public AcademicAdvisor AcademicAdvisor { get; set; }
    }
}