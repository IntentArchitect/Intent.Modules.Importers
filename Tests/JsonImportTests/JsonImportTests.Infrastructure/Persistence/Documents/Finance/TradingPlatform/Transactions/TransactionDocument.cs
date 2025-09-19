using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class TransactionDocument : ITransactionDocument, ICosmosDBDocument<Transaction, TransactionDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string TransactionId { get; set; } = default!;
        public Guid PortfolioId { get; set; }
        public Guid AssetId { get; set; }
        public Guid OrderId { get; set; }
        public string TransactionType { get; set; } = default!;
        public string TransactionStatus { get; set; } = default!;
        public DateTime TradeDate { get; set; }
        public DateTime SettlementDate { get; set; }
        public DateTime BookingDate { get; set; }
        public TaxisDocument Taxes { get; set; } = default!;
        ITaxisDocument ITransactionDocument.Taxes => Taxes;
        public SettlementDetailDocument SettlementDetails { get; set; } = default!;
        ISettlementDetailDocument ITransactionDocument.SettlementDetails => SettlementDetails;
        public RiskDatumDocument RiskData { get; set; } = default!;
        IRiskDatumDocument ITransactionDocument.RiskData => RiskData;
        public List<RelatedTransactionDocument> RelatedTransactions { get; set; } = default!;
        IReadOnlyList<IRelatedTransactionDocument> ITransactionDocument.RelatedTransactions => RelatedTransactions;
        public QuantityDocument Quantity { get; set; } = default!;
        IQuantityDocument ITransactionDocument.Quantity => Quantity;
        public TransactionPricingDocument Pricing { get; set; } = default!;
        ITransactionPricingDocument ITransactionDocument.Pricing => Pricing;
        public OrderDetailDocument OrderDetails { get; set; } = default!;
        IOrderDetailDocument ITransactionDocument.OrderDetails => OrderDetails;
        public InstrumentDocument Instrument { get; set; } = default!;
        IInstrumentDocument ITransactionDocument.Instrument => Instrument;
        public FeeDocument Fees { get; set; } = default!;
        IFeeDocument ITransactionDocument.Fees => Fees;
        public CounterpartyDocument Counterparty { get; set; } = default!;
        ICounterpartyDocument ITransactionDocument.Counterparty => Counterparty;
        public ComplianceDocument Compliance { get; set; } = default!;
        IComplianceDocument ITransactionDocument.Compliance => Compliance;
        public AuditTrailDocument AuditTrail { get; set; } = default!;
        IAuditTrailDocument ITransactionDocument.AuditTrail => AuditTrail;
        public List<AmendmentDocument> Amendments { get; set; } = default!;
        IReadOnlyList<IAmendmentDocument> ITransactionDocument.Amendments => Amendments;
        public List<AllocationDocument> Allocations { get; set; } = default!;
        IReadOnlyList<IAllocationDocument> ITransactionDocument.Allocations => Allocations;

        public Transaction ToEntity(Transaction? entity = default)
        {
            entity ??= new Transaction();

            entity.Id = Guid.Parse(Id);
            entity.TransactionId = TransactionId ?? throw new Exception($"{nameof(entity.TransactionId)} is null");
            entity.PortfolioId = PortfolioId;
            entity.AssetId = AssetId;
            entity.OrderId = OrderId;
            entity.TransactionType = TransactionType ?? throw new Exception($"{nameof(entity.TransactionType)} is null");
            entity.TransactionStatus = TransactionStatus ?? throw new Exception($"{nameof(entity.TransactionStatus)} is null");
            entity.TradeDate = TradeDate;
            entity.SettlementDate = SettlementDate;
            entity.BookingDate = BookingDate;
            entity.Taxes = Taxes.ToEntity() ?? throw new Exception($"{nameof(entity.Taxes)} is null");
            entity.SettlementDetails = SettlementDetails.ToEntity() ?? throw new Exception($"{nameof(entity.SettlementDetails)} is null");
            entity.RiskData = RiskData.ToEntity() ?? throw new Exception($"{nameof(entity.RiskData)} is null");
            entity.RelatedTransactions = RelatedTransactions.Select(x => x.ToEntity()).ToList();
            entity.Quantity = Quantity.ToEntity() ?? throw new Exception($"{nameof(entity.Quantity)} is null");
            entity.Pricing = Pricing.ToEntity() ?? throw new Exception($"{nameof(entity.Pricing)} is null");
            entity.OrderDetails = OrderDetails.ToEntity() ?? throw new Exception($"{nameof(entity.OrderDetails)} is null");
            entity.Instrument = Instrument.ToEntity() ?? throw new Exception($"{nameof(entity.Instrument)} is null");
            entity.Fees = Fees.ToEntity() ?? throw new Exception($"{nameof(entity.Fees)} is null");
            entity.Counterparty = Counterparty.ToEntity() ?? throw new Exception($"{nameof(entity.Counterparty)} is null");
            entity.Compliance = Compliance.ToEntity() ?? throw new Exception($"{nameof(entity.Compliance)} is null");
            entity.AuditTrail = AuditTrail.ToEntity() ?? throw new Exception($"{nameof(entity.AuditTrail)} is null");
            entity.Amendments = Amendments.Select(x => x.ToEntity()).ToList();
            entity.Allocations = Allocations.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public TransactionDocument PopulateFromEntity(Transaction entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            TransactionId = entity.TransactionId;
            PortfolioId = entity.PortfolioId;
            AssetId = entity.AssetId;
            OrderId = entity.OrderId;
            TransactionType = entity.TransactionType;
            TransactionStatus = entity.TransactionStatus;
            TradeDate = entity.TradeDate;
            SettlementDate = entity.SettlementDate;
            BookingDate = entity.BookingDate;
            Taxes = TaxisDocument.FromEntity(entity.Taxes)!;
            SettlementDetails = SettlementDetailDocument.FromEntity(entity.SettlementDetails)!;
            RiskData = RiskDatumDocument.FromEntity(entity.RiskData)!;
            RelatedTransactions = entity.RelatedTransactions.Select(x => RelatedTransactionDocument.FromEntity(x)!).ToList();
            Quantity = QuantityDocument.FromEntity(entity.Quantity)!;
            Pricing = TransactionPricingDocument.FromEntity(entity.Pricing)!;
            OrderDetails = OrderDetailDocument.FromEntity(entity.OrderDetails)!;
            Instrument = InstrumentDocument.FromEntity(entity.Instrument)!;
            Fees = FeeDocument.FromEntity(entity.Fees)!;
            Counterparty = CounterpartyDocument.FromEntity(entity.Counterparty)!;
            Compliance = ComplianceDocument.FromEntity(entity.Compliance)!;
            AuditTrail = AuditTrailDocument.FromEntity(entity.AuditTrail)!;
            Amendments = entity.Amendments.Select(x => AmendmentDocument.FromEntity(x)!).ToList();
            Allocations = entity.Allocations.Select(x => AllocationDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static TransactionDocument? FromEntity(Transaction? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new TransactionDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}