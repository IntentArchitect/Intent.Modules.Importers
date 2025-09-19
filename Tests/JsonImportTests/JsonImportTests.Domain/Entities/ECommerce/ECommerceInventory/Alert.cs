using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class Alert
    {
        private string? _id;

        public Alert()
        {
            Id = null!;
            Type = null!;
            Message = null!;
            Severity = null!;
        }

        public string Id
        {
            get => _id ??= Guid.NewGuid().ToString();
            set => _id = value;
        }

        public string Type { get; set; }

        public string Message { get; set; }

        public string Severity { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsAcknowledged { get; set; }
    }
}