using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions
{
    public class ComplianceCheck
    {
        private string? _id;

        public ComplianceCheck()
        {
            Id = null!;
            CheckType = null!;
            Status = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string CheckType { get; set; }

        public string Status { get; set; }

        public DateTime CheckedAt { get; set; }
    }
}