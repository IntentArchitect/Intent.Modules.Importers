using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical
{
    public interface IVisitDetailsTreatmentPlanDocument
    {
        string Instructions { get; }
        string FollowUpInstructions { get; }
        bool NextAppointmentRecommended { get; }
        IReadOnlyList<ITreatmentPlanMedicationDocument> Medications { get; }
    }
}