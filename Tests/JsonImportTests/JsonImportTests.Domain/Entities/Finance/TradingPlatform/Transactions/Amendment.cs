using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Amendment
    {
        private Guid? _amendmentId;

        public Amendment()
        {
            AmendmentType = null!;
            AmendmentReason = null!;
            OriginalValue = null!;
            NewValue = null!;
            AmendedBy = null!;
        }

        public Guid AmendmentId
        {
            get => _amendmentId ??= Guid.NewGuid();
            set => _amendmentId = value;
        }

        public string AmendmentType { get; set; }

        public string AmendmentReason { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        public string AmendedBy { get; set; }

        public DateTime AmendedAt { get; set; }
    }
}