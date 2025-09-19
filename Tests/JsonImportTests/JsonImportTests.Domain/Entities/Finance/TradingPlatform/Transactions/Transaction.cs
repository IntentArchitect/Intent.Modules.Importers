using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Transaction
    {
        private Guid? _id;

        public Transaction()
        {
            TransactionId = null!;
            TransactionType = null!;
            TransactionStatus = null!;
            Taxes = null!;
            SettlementDetails = null!;
            RiskData = null!;
            Quantity = null!;
            Pricing = null!;
            OrderDetails = null!;
            Instrument = null!;
            Fees = null!;
            Counterparty = null!;
            Compliance = null!;
            AuditTrail = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string TransactionId { get; set; }

        public Guid PortfolioId { get; set; }

        public Guid AssetId { get; set; }

        public Guid OrderId { get; set; }

        public string TransactionType { get; set; }

        public string TransactionStatus { get; set; }

        public DateTime TradeDate { get; set; }

        public DateTime SettlementDate { get; set; }

        public DateTime BookingDate { get; set; }

        public Taxis Taxes { get; set; }

        public SettlementDetail SettlementDetails { get; set; }

        public RiskDatum RiskData { get; set; }

        public ICollection<RelatedTransaction> RelatedTransactions { get; set; } = [];

        public Quantity Quantity { get; set; }

        public TransactionPricing Pricing { get; set; }

        public OrderDetail OrderDetails { get; set; }

        public Instrument Instrument { get; set; }

        public Fee Fees { get; set; }

        public Counterparty Counterparty { get; set; }

        public Compliance Compliance { get; set; }

        public AuditTrail AuditTrail { get; set; }

        public ICollection<Amendment> Amendments { get; set; } = [];

        public ICollection<Allocation> Allocations { get; set; } = [];
    }
}