using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface IComplianceDocument
    {
        IReadOnlyList<object> TradeRestrictions { get; }
        bool RequiresApproval { get; }
        object ApprovedBy { get; }
        object ApprovalTime { get; }
        IReadOnlyList<IComplianceCheckDocument> ComplianceChecks { get; }
    }
}