using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class CounselingService
    {
        public CounselingService()
        {
            CounselorId = null!;
            LastAppointment = null!;
        }

        public bool IsReceivingServices { get; set; }

        public object CounselorId { get; set; }

        public object LastAppointment { get; set; }
    }
}