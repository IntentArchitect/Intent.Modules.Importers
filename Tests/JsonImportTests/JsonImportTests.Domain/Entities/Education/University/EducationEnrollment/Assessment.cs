using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class Assessment
    {
        private Guid? _assessmentId;

        public Assessment()
        {
            Type = null!;
            Name = null!;
            Comments = null!;
        }

        public Guid AssessmentId
        {
            get => _assessmentId ??= Guid.NewGuid();
            set => _assessmentId = value;
        }

        public string Type { get; set; }

        public string Name { get; set; }

        public decimal MaxPoints { get; set; }

        public decimal EarnedPoints { get; set; }

        public decimal Percentage { get; set; }

        public decimal Weight { get; set; }

        public DateTime DueDate { get; set; }

        public DateTime SubmittedDate { get; set; }

        public DateTime GradedDate { get; set; }

        public string Comments { get; set; }

        public ICollection<Rubric> Rubric { get; set; } = [];
    }
}