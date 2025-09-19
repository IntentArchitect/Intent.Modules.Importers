using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class PaymentPlanDocument : IPaymentPlanDocument
    {
        public string PlanType { get; set; } = default!;
        public decimal MonthlyAmount { get; set; }
        public DateTime NextDueDate { get; set; }

        public PaymentPlan ToEntity(PaymentPlan? entity = default)
        {
            entity ??= new PaymentPlan();

            entity.PlanType = PlanType ?? throw new Exception($"{nameof(entity.PlanType)} is null");
            entity.MonthlyAmount = MonthlyAmount;
            entity.NextDueDate = NextDueDate;

            return entity;
        }

        public PaymentPlanDocument PopulateFromEntity(PaymentPlan entity)
        {
            PlanType = entity.PlanType;
            MonthlyAmount = entity.MonthlyAmount;
            NextDueDate = entity.NextDueDate;

            return this;
        }

        public static PaymentPlanDocument? FromEntity(PaymentPlan? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PaymentPlanDocument().PopulateFromEntity(entity);
        }
    }
}