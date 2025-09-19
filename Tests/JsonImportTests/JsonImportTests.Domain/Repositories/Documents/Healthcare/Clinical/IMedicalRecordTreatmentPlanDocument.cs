using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IMedicalRecordTreatmentPlanDocument
    {
        string Id { get; }
        Guid PlanId { get; }
        string Description { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
        string Status { get; }
        IReadOnlyList<string> Goals { get; }
        IReadOnlyList<IInterventionDocument> Interventions { get; }
    }
}