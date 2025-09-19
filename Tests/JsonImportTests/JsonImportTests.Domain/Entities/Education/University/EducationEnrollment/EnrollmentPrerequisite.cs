using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class EnrollmentPrerequisite
    {
        public bool AllPrerequisitesMet { get; set; }

        public ICollection<Waiver> Waivers { get; set; } = [];

        public ICollection<PrerequisiteCheck> PrerequisiteChecks { get; set; } = [];
    }
}