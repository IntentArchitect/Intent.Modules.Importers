using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Education.University.Students
{
    public class PaymentPlan
    {
        public PaymentPlan()
        {
            PlanType = null!;
        }

        public string PlanType { get; set; }

        public decimal MonthlyAmount { get; set; }

        public DateTime NextDueDate { get; set; }
    }
}