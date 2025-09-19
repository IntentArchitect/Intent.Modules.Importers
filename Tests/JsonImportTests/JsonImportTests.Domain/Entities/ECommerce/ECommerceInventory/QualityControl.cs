using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class QualityControl
    {
        public QualityControl()
        {
            BatchNumber = null!;
            InspectionResults = null!;
        }

        public DateTime LastInspectionDate { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string BatchNumber { get; set; }

        public IList<string> SerialNumbers { get; set; } = [];

        public InspectionResult InspectionResults { get; set; }
    }
}