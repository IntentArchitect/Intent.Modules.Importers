using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class RiskDatumDocument : IRiskDatumDocument
    {
        public decimal VaRImpact { get; set; }
        public decimal DeltaEquivalent { get; set; }
        public decimal NotionalAmount { get; set; }
        public string RiskClass { get; set; } = default!;
        public decimal ConcentrationLimit { get; set; }
        public decimal PositionLimit { get; set; }

        public RiskDatum ToEntity(RiskDatum? entity = default)
        {
            entity ??= new RiskDatum();

            entity.VaRImpact = VaRImpact;
            entity.DeltaEquivalent = DeltaEquivalent;
            entity.NotionalAmount = NotionalAmount;
            entity.RiskClass = RiskClass ?? throw new Exception($"{nameof(entity.RiskClass)} is null");
            entity.ConcentrationLimit = ConcentrationLimit;
            entity.PositionLimit = PositionLimit;

            return entity;
        }

        public RiskDatumDocument PopulateFromEntity(RiskDatum entity)
        {
            VaRImpact = entity.VaRImpact;
            DeltaEquivalent = entity.DeltaEquivalent;
            NotionalAmount = entity.NotionalAmount;
            RiskClass = entity.RiskClass;
            ConcentrationLimit = entity.ConcentrationLimit;
            PositionLimit = entity.PositionLimit;

            return this;
        }

        public static RiskDatumDocument? FromEntity(RiskDatum? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RiskDatumDocument().PopulateFromEntity(entity);
        }
    }
}