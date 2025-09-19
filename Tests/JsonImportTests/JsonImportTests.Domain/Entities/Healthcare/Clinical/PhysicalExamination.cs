using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Healthcare.Clinical
{
    public class PhysicalExamination
    {
        public PhysicalExamination()
        {
            General = null!;
            VitalSigns = null!;
            SystemicFindings = null!;
        }

        public string General { get; set; }

        public PhysicalExaminationVitalSign VitalSigns { get; set; }

        public SystemicFinding SystemicFindings { get; set; }
    }
}