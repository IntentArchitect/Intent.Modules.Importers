using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions
{
    public interface ITransactionDocument
    {
        string Id { get; }
        string TransactionId { get; }
        Guid PortfolioId { get; }
        Guid AssetId { get; }
        Guid OrderId { get; }
        string TransactionType { get; }
        string TransactionStatus { get; }
        DateTime TradeDate { get; }
        DateTime SettlementDate { get; }
        DateTime BookingDate { get; }
        ITaxisDocument Taxes { get; }
        ISettlementDetailDocument SettlementDetails { get; }
        IRiskDatumDocument RiskData { get; }
        IReadOnlyList<IRelatedTransactionDocument> RelatedTransactions { get; }
        IQuantityDocument Quantity { get; }
        ITransactionPricingDocument Pricing { get; }
        IOrderDetailDocument OrderDetails { get; }
        IInstrumentDocument Instrument { get; }
        IFeeDocument Fees { get; }
        ICounterpartyDocument Counterparty { get; }
        IComplianceDocument Compliance { get; }
        IAuditTrailDocument AuditTrail { get; }
        IReadOnlyList<IAmendmentDocument> Amendments { get; }
        IReadOnlyList<IAllocationDocument> Allocations { get; }
    }
}