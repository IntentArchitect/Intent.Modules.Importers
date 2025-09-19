using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.EducationEnrollment
{
    public class Participation
    {
        public Participation()
        {
            ParticipationGrade = null!;
            ParticipationComments = null!;
            EngagementLevel = null!;
        }

        public string ParticipationGrade { get; set; }

        public decimal ParticipationPoints { get; set; }

        public decimal MaxParticipationPoints { get; set; }

        public string ParticipationComments { get; set; }

        public string EngagementLevel { get; set; }
    }
}