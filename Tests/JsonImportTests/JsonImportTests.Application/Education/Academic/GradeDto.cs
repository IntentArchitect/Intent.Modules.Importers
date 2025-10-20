using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonImportTests.Application.Education.Academic
{
    public class GradeDto
    {
        public GradeDto()
        {
            AssessmentType = null!;
        }

        public string AssessmentType { get; set; }
        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public DateTime GradedDate { get; set; }

        public static GradeDto Create(string assessmentType, decimal score, decimal maxScore, DateTime gradedDate)
        {
            return new GradeDto
            {
                AssessmentType = assessmentType,
                Score = score,
                MaxScore = maxScore,
                GradedDate = gradedDate
            };
        }
    }
}