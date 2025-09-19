using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class AnalystDocument : IAnalystDocument
    {
        public string Consensus { get; set; } = default!;
        public decimal TargetPrice { get; set; }
        public decimal PriceTargetHigh { get; set; }
        public decimal PriceTargetLow { get; set; }
        public RecommendationDocument Recommendations { get; set; } = default!;
        IRecommendationDocument IAnalystDocument.Recommendations => Recommendations;

        public Analyst ToEntity(Analyst? entity = default)
        {
            entity ??= new Analyst();

            entity.Consensus = Consensus ?? throw new Exception($"{nameof(entity.Consensus)} is null");
            entity.TargetPrice = TargetPrice;
            entity.PriceTargetHigh = PriceTargetHigh;
            entity.PriceTargetLow = PriceTargetLow;
            entity.Recommendations = Recommendations.ToEntity() ?? throw new Exception($"{nameof(entity.Recommendations)} is null");

            return entity;
        }

        public AnalystDocument PopulateFromEntity(Analyst entity)
        {
            Consensus = entity.Consensus;
            TargetPrice = entity.TargetPrice;
            PriceTargetHigh = entity.PriceTargetHigh;
            PriceTargetLow = entity.PriceTargetLow;
            Recommendations = RecommendationDocument.FromEntity(entity.Recommendations)!;

            return this;
        }

        public static AnalystDocument? FromEntity(Analyst? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AnalystDocument().PopulateFromEntity(entity);
        }
    }
}