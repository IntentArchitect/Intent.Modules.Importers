using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.ECommerceInventory;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.ECommerceInventory;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.ECommerceInventory
{
    internal class InspectionResultDocument : IInspectionResultDocument
    {
        public string Grade { get; set; } = default!;
        public decimal DefectCount { get; set; }
        public string Notes { get; set; } = default!;
        public string InspectedBy { get; set; } = default!;

        public InspectionResult ToEntity(InspectionResult? entity = default)
        {
            entity ??= new InspectionResult();

            entity.Grade = Grade ?? throw new Exception($"{nameof(entity.Grade)} is null");
            entity.DefectCount = DefectCount;
            entity.Notes = Notes ?? throw new Exception($"{nameof(entity.Notes)} is null");
            entity.InspectedBy = InspectedBy ?? throw new Exception($"{nameof(entity.InspectedBy)} is null");

            return entity;
        }

        public InspectionResultDocument PopulateFromEntity(InspectionResult entity)
        {
            Grade = entity.Grade;
            DefectCount = entity.DefectCount;
            Notes = entity.Notes;
            InspectedBy = entity.InspectedBy;

            return this;
        }

        public static InspectionResultDocument? FromEntity(InspectionResult? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new InspectionResultDocument().PopulateFromEntity(entity);
        }
    }
}