using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class InventoryItem
    {
        private Guid? _id;

        public InventoryItem()
        {
            SKU = null!;
            UpdatedBy = null!;
            StockLevels = null!;
            QualityControl = null!;
            Location = null!;
            CostTracking = null!;
        }

        public Guid Id
        {
            get => _id ??= Guid.NewGuid();
            set => _id = value;
        }

        public Guid ProductId { get; set; }

        public Guid VariantId { get; set; }

        public string SKU { get; set; }

        public DateTime LastUpdated { get; set; }

        public string UpdatedBy { get; set; }

        public decimal Version { get; set; }

        public StockLevel StockLevels { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = [];

        public QualityControl QualityControl { get; set; }

        public ICollection<MovementHistory> MovementHistory { get; set; } = [];

        public InventoryItemLocation Location { get; set; }

        public CostTracking CostTracking { get; set; }

        public ICollection<Alert> Alerts { get; set; } = [];
    }
}