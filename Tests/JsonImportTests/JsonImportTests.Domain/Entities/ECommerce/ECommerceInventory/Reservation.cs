using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class Reservation
    {
        private Guid? _id;

        public Reservation()
        {
            Status = null!;
            ReservedBy = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public Guid OrderId { get; set; }

        public decimal Quantity { get; set; }

        public DateTime ReservedDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Status { get; set; }

        public string ReservedBy { get; set; }
    }
}