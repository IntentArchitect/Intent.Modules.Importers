using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class MovementHistory
    {
        private Guid? _id;

        public MovementHistory()
        {
            Type = null!;
            Reason = null!;
            User = null!;
            Notes = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public string Type { get; set; }

        public decimal Quantity { get; set; }

        public string Reason { get; set; }

        public Guid ReferenceId { get; set; }

        public DateTime Date { get; set; }

        public string User { get; set; }

        public string Notes { get; set; }

        public decimal UnitCost { get; set; }
    }
}