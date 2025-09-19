using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class AuditTrail
    {
        public AuditTrail()
        {
            CreatedBy = null!;
            LastModifiedBy = null!;
            Source = null!;
        }

        public string CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime LastModifiedAt { get; set; }

        public decimal Version { get; set; }

        public string Source { get; set; }
    }
}