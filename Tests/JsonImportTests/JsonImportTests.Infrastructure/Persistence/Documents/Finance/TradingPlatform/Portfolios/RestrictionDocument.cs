using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class RestrictionDocument : IRestrictionDocument
    {
        public decimal MaxPositionSize { get; set; }
        public List<string> ProhibitedAssets { get; set; } = default!;
        IReadOnlyList<string> IRestrictionDocument.ProhibitedAssets => ProhibitedAssets;
        public decimal MinCashPercentage { get; set; }
        public decimal MaxSectorConcentration { get; set; }

        public Restriction ToEntity(Restriction? entity = default)
        {
            entity ??= new Restriction();

            entity.MaxPositionSize = MaxPositionSize;
            entity.ProhibitedAssets = ProhibitedAssets ?? throw new Exception($"{nameof(entity.ProhibitedAssets)} is null");
            entity.MinCashPercentage = MinCashPercentage;
            entity.MaxSectorConcentration = MaxSectorConcentration;

            return entity;
        }

        public RestrictionDocument PopulateFromEntity(Restriction entity)
        {
            MaxPositionSize = entity.MaxPositionSize;
            ProhibitedAssets = entity.ProhibitedAssets.ToList();
            MinCashPercentage = entity.MinCashPercentage;
            MaxSectorConcentration = entity.MaxSectorConcentration;

            return this;
        }

        public static RestrictionDocument? FromEntity(Restriction? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RestrictionDocument().PopulateFromEntity(entity);
        }
    }
}