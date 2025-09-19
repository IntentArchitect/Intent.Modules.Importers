using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class VisitDetail
    {
        public VisitDetail()
        {
            VitalSigns = null!;
            TreatmentPlan = null!;
        }

        public VisitDetailsVitalSign VitalSigns { get; set; }

        public VisitDetailsTreatmentPlan TreatmentPlan { get; set; }

        public ICollection<Symptom> Symptoms { get; set; } = [];

        public ICollection<VisitDetailsDiagnosis> Diagnosis { get; set; } = [];
    }
}