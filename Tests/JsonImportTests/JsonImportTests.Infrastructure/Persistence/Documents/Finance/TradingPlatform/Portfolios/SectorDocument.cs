using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class SectorDocument : ISectorDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Percentage { get; set; }
        public decimal Value { get; set; }

        public Sector ToEntity(Sector? entity = default)
        {
            entity ??= new Sector();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Percentage = Percentage;
            entity.Value = Value;

            return entity;
        }

        public SectorDocument PopulateFromEntity(Sector entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Percentage = entity.Percentage;
            Value = entity.Value;

            return this;
        }

        public static SectorDocument? FromEntity(Sector? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SectorDocument().PopulateFromEntity(entity);
        }
    }
}