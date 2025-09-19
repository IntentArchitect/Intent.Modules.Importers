using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "2.0")]

namespace JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory
{
    public class InspectionResult
    {
        public InspectionResult()
        {
            Grade = null!;
            Notes = null!;
            InspectedBy = null!;
        }

        public string Grade { get; set; }

        public decimal DefectCount { get; set; }

        public string Notes { get; set; }

        public string InspectedBy { get; set; }
    }
}