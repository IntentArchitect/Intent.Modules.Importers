using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IPaymentPlanDocument
    {
        string PlanType { get; }
        decimal MonthlyAmount { get; }
        DateTime NextDueDate { get; }
    }
}