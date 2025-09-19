using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class PropertyDocument : IPropertyDocument
    {
        public string Make { get; set; } = default!;
        public string Model { get; set; } = default!;
        public decimal Year { get; set; }
        public string VIN { get; set; } = default!;

        public Property ToEntity(Property? entity = default)
        {
            entity ??= new Property();

            entity.Make = Make ?? throw new Exception($"{nameof(entity.Make)} is null");
            entity.Model = Model ?? throw new Exception($"{nameof(entity.Model)} is null");
            entity.Year = Year;
            entity.VIN = VIN ?? throw new Exception($"{nameof(entity.VIN)} is null");

            return entity;
        }

        public PropertyDocument PopulateFromEntity(Property entity)
        {
            Make = entity.Make;
            Model = entity.Model;
            Year = entity.Year;
            VIN = entity.VIN;

            return this;
        }

        public static PropertyDocument? FromEntity(Property? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PropertyDocument().PopulateFromEntity(entity);
        }
    }
}