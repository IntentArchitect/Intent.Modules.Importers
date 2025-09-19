using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class ClinicalDatum
    {
        public ClinicalDatum()
        {
            ChiefComplaint = null!;
            HistoryOfPresentIllness = null!;
            ReviewOfSystems = null!;
            PhysicalExamination = null!;
        }

        public string ChiefComplaint { get; set; }

        public string HistoryOfPresentIllness { get; set; }

        public ReviewOfSystem ReviewOfSystems { get; set; }

        public PhysicalExamination PhysicalExamination { get; set; }
    }
}