using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class SystemicFindingDocument : ISystemicFindingDocument
    {
        public string Head { get; set; } = default!;
        public string Neck { get; set; } = default!;
        public string Chest { get; set; } = default!;
        public string Abdomen { get; set; } = default!;
        public string Extremities { get; set; } = default!;

        public SystemicFinding ToEntity(SystemicFinding? entity = default)
        {
            entity ??= new SystemicFinding();

            entity.Head = Head ?? throw new Exception($"{nameof(entity.Head)} is null");
            entity.Neck = Neck ?? throw new Exception($"{nameof(entity.Neck)} is null");
            entity.Chest = Chest ?? throw new Exception($"{nameof(entity.Chest)} is null");
            entity.Abdomen = Abdomen ?? throw new Exception($"{nameof(entity.Abdomen)} is null");
            entity.Extremities = Extremities ?? throw new Exception($"{nameof(entity.Extremities)} is null");

            return entity;
        }

        public SystemicFindingDocument PopulateFromEntity(SystemicFinding entity)
        {
            Head = entity.Head;
            Neck = entity.Neck;
            Chest = entity.Chest;
            Abdomen = entity.Abdomen;
            Extremities = entity.Extremities;

            return this;
        }

        public static SystemicFindingDocument? FromEntity(SystemicFinding? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SystemicFindingDocument().PopulateFromEntity(entity);
        }
    }
}