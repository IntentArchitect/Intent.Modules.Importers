using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class RelatedTransaction
    {
        private Guid? _relatedTransactionId;

        public RelatedTransaction()
        {
            RelationType = null!;
            Description = null!;
        }

        public Guid RelatedTransactionId
        {
            get => _relatedTransactionId ??= Guid.NewGuid();
            set => _relatedTransactionId = value;
        }

        public string RelationType { get; set; }

        public string Description { get; set; }
    }
}