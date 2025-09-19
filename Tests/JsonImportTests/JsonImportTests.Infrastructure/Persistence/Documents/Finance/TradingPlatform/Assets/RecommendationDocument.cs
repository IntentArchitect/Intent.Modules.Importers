using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class RecommendationDocument : IRecommendationDocument
    {
        public decimal StrongBuy { get; set; }
        public decimal Buy { get; set; }
        public decimal Hold { get; set; }
        public decimal Sell { get; set; }
        public decimal StrongSell { get; set; }

        public Recommendation ToEntity(Recommendation? entity = default)
        {
            entity ??= new Recommendation();

            entity.StrongBuy = StrongBuy;
            entity.Buy = Buy;
            entity.Hold = Hold;
            entity.Sell = Sell;
            entity.StrongSell = StrongSell;

            return entity;
        }

        public RecommendationDocument PopulateFromEntity(Recommendation entity)
        {
            StrongBuy = entity.StrongBuy;
            Buy = entity.Buy;
            Hold = entity.Hold;
            Sell = entity.Sell;
            StrongSell = entity.StrongSell;

            return this;
        }

        public static RecommendationDocument? FromEntity(Recommendation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RecommendationDocument().PopulateFromEntity(entity);
        }
    }
}