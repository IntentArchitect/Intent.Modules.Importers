using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface ISpecialCircumstanceDocument
    {
        ILateEnrollmentDocument LateEnrollment { get; }
        IIncompleteStatusDocument IncompleteStatus { get; }
        IAuditStatusDocument AuditStatus { get; }
    }
}