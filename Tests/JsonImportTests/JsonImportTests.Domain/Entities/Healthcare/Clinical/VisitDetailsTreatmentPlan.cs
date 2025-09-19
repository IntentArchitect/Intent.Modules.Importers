using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class VisitDetailsTreatmentPlan
    {
        public VisitDetailsTreatmentPlan()
        {
            Instructions = null!;
            FollowUpInstructions = null!;
        }

        public string Instructions { get; set; }

        public string FollowUpInstructions { get; set; }

        public bool NextAppointmentRecommended { get; set; }

        public ICollection<TreatmentPlanMedication> Medications { get; set; } = [];
    }
}