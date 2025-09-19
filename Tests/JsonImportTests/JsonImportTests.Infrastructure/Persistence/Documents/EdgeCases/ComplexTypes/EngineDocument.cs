using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class EngineDocument : IEngineDocument
    {
        public string Type { get; set; } = default!;
        public decimal Displacement { get; set; }
        public decimal Horsepower { get; set; }
        public bool Turbo { get; set; }

        public Engine ToEntity(Engine? entity = default)
        {
            entity ??= new Engine();

            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Displacement = Displacement;
            entity.Horsepower = Horsepower;
            entity.Turbo = Turbo;

            return entity;
        }

        public EngineDocument PopulateFromEntity(Engine entity)
        {
            Type = entity.Type;
            Displacement = entity.Displacement;
            Horsepower = entity.Horsepower;
            Turbo = entity.Turbo;

            return this;
        }

        public static EngineDocument? FromEntity(Engine? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EngineDocument().PopulateFromEntity(entity);
        }
    }
}