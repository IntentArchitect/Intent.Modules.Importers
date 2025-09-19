using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios
{
    public class Owner
    {
        public Owner()
        {
            Name = null!;
            Email = null!;
            Phone = null!;
        }

        public Guid CustomerId { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}