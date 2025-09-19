using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IWaiverDocument
    {
        Guid WaiverId { get; }
        string PrerequisiteWaived { get; }
        string WaiverReason { get; }
        string ApprovedBy { get; }
        DateTime ApprovalDate { get; }
    }
}