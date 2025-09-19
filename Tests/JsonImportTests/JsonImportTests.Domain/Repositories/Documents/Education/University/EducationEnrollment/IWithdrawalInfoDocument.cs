using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IWithdrawalInfoDocument
    {
        object WithdrawalDate { get; }
        object WithdrawalReason { get; }
        object WithdrawalType { get; }
        object RefundPercentage { get; }
        object WithdrawnBy { get; }
        object AcademicImpact { get; }
    }
}