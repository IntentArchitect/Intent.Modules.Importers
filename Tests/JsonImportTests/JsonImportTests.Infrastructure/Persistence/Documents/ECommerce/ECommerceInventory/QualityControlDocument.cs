using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class QualityControlDocument : IQualityControlDocument
    {
        public DateTime LastInspectionDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string BatchNumber { get; set; } = default!;
        public List<string> SerialNumbers { get; set; } = default!;
        IReadOnlyList<string> IQualityControlDocument.SerialNumbers => SerialNumbers;
        public InspectionResultDocument InspectionResults { get; set; } = default!;
        IInspectionResultDocument IQualityControlDocument.InspectionResults => InspectionResults;

        public QualityControl ToEntity(QualityControl? entity = default)
        {
            entity ??= new QualityControl();

            entity.LastInspectionDate = LastInspectionDate;
            entity.ExpirationDate = ExpirationDate;
            entity.BatchNumber = BatchNumber ?? throw new Exception($"{nameof(entity.BatchNumber)} is null");
            entity.SerialNumbers = SerialNumbers ?? throw new Exception($"{nameof(entity.SerialNumbers)} is null");
            entity.InspectionResults = InspectionResults.ToEntity() ?? throw new Exception($"{nameof(entity.InspectionResults)} is null");

            return entity;
        }

        public QualityControlDocument PopulateFromEntity(QualityControl entity)
        {
            LastInspectionDate = entity.LastInspectionDate;
            ExpirationDate = entity.ExpirationDate;
            BatchNumber = entity.BatchNumber;
            SerialNumbers = entity.SerialNumbers.ToList();
            InspectionResults = InspectionResultDocument.FromEntity(entity.InspectionResults)!;

            return this;
        }

        public static QualityControlDocument? FromEntity(QualityControl? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new QualityControlDocument().PopulateFromEntity(entity);
        }
    }
}