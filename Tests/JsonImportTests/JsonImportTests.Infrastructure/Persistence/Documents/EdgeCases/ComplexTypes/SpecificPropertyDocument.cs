using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class SpecificPropertyDocument : ISpecificPropertyDocument
    {
        public string VehicleType { get; set; } = default!;
        public decimal Doors { get; set; }
        public string FuelType { get; set; } = default!;
        public string Transmission { get; set; } = default!;
        public List<string> Features { get; set; } = default!;
        IReadOnlyList<string> ISpecificPropertyDocument.Features => Features;
        public SafetyDocument Safety { get; set; } = default!;
        ISafetyDocument ISpecificPropertyDocument.Safety => Safety;
        public EngineDocument Engine { get; set; } = default!;
        IEngineDocument ISpecificPropertyDocument.Engine => Engine;

        public SpecificProperty ToEntity(SpecificProperty? entity = default)
        {
            entity ??= new SpecificProperty();

            entity.VehicleType = VehicleType ?? throw new Exception($"{nameof(entity.VehicleType)} is null");
            entity.Doors = Doors;
            entity.FuelType = FuelType ?? throw new Exception($"{nameof(entity.FuelType)} is null");
            entity.Transmission = Transmission ?? throw new Exception($"{nameof(entity.Transmission)} is null");
            entity.Features = Features ?? throw new Exception($"{nameof(entity.Features)} is null");
            entity.Safety = Safety.ToEntity() ?? throw new Exception($"{nameof(entity.Safety)} is null");
            entity.Engine = Engine.ToEntity() ?? throw new Exception($"{nameof(entity.Engine)} is null");

            return entity;
        }

        public SpecificPropertyDocument PopulateFromEntity(SpecificProperty entity)
        {
            VehicleType = entity.VehicleType;
            Doors = entity.Doors;
            FuelType = entity.FuelType;
            Transmission = entity.Transmission;
            Features = entity.Features.ToList();
            Safety = SafetyDocument.FromEntity(entity.Safety)!;
            Engine = EngineDocument.FromEntity(entity.Engine)!;

            return this;
        }

        public static SpecificPropertyDocument? FromEntity(SpecificProperty? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SpecificPropertyDocument().PopulateFromEntity(entity);
        }
    }
}