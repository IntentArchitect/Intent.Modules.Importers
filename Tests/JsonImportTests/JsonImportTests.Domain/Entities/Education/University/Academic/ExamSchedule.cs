using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Academic
{
    public class ExamSchedule
    {
        public ExamSchedule()
        {
            Location = null!;
        }

        public DateTime MidtermDate { get; set; }

        public DateTime FinalExamDate { get; set; }

        public string Location { get; set; }
    }
}