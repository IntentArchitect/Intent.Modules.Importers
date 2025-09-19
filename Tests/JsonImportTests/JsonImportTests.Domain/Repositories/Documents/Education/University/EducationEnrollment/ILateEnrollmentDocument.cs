using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface ILateEnrollmentDocument
    {
        bool IsLateEnrollment { get; }
        decimal LateEnrollmentFee { get; }
        bool ApprovalRequired { get; }
        object ApprovedBy { get; }
    }
}