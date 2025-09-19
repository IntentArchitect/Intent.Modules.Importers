using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class Rubric
    {
        private string? _id;

        public Rubric()
        {
            Id = null!;
            CriteriaName = null!;
            Comments = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public Guid CriteriaId { get; set; }

        public string CriteriaName { get; set; }

        public decimal MaxPoints { get; set; }

        public decimal EarnedPoints { get; set; }

        public string Comments { get; set; }
    }
}