using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class MetadataMapDocument : IMetadataMapDocument
    {
        public string Key1 { get; set; } = default!;
        public decimal Key2 { get; set; }
        public bool Key3 { get; set; }
        public object Key4 { get; set; } = default!;
        public Key5Document Key5 { get; set; } = default!;
        IKey5Document IMetadataMapDocument.Key5 => Key5;

        public MetadataMap ToEntity(MetadataMap? entity = default)
        {
            entity ??= new MetadataMap();

            entity.Key1 = Key1 ?? throw new Exception($"{nameof(entity.Key1)} is null");
            entity.Key2 = Key2;
            entity.Key3 = Key3;
            entity.Key4 = Key4 ?? throw new Exception($"{nameof(entity.Key4)} is null");
            entity.Key5 = Key5.ToEntity() ?? throw new Exception($"{nameof(entity.Key5)} is null");

            return entity;
        }

        public MetadataMapDocument PopulateFromEntity(MetadataMap entity)
        {
            Key1 = entity.Key1;
            Key2 = entity.Key2;
            Key3 = entity.Key3;
            Key4 = entity.Key4;
            Key5 = Key5Document.FromEntity(entity.Key5)!;

            return this;
        }

        public static MetadataMapDocument? FromEntity(MetadataMap? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new MetadataMapDocument().PopulateFromEntity(entity);
        }
    }
}