using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class Compliance
    {
        public Compliance()
        {
            ApprovedBy = null!;
            ApprovalTime = null!;
        }

        public ICollection<object> TradeRestrictions { get; set; } = [];

        public bool RequiresApproval { get; set; }

        public object ApprovedBy { get; set; }

        public object ApprovalTime { get; set; }

        public ICollection<ComplianceCheck> ComplianceChecks { get; set; } = [];
    }
}