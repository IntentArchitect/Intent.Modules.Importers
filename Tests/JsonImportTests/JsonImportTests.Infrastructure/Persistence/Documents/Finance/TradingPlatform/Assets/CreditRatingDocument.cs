using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Assets;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Assets;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Assets
{
    internal class CreditRatingDocument : ICreditRatingDocument
    {
        public string SP { get; set; } = default!;
        public string Moody { get; set; } = default!;
        public string Fitch { get; set; } = default!;

        public CreditRating ToEntity(CreditRating? entity = default)
        {
            entity ??= new CreditRating();

            entity.SP = SP ?? throw new Exception($"{nameof(entity.SP)} is null");
            entity.Moody = Moody ?? throw new Exception($"{nameof(entity.Moody)} is null");
            entity.Fitch = Fitch ?? throw new Exception($"{nameof(entity.Fitch)} is null");

            return entity;
        }

        public CreditRatingDocument PopulateFromEntity(CreditRating entity)
        {
            SP = entity.SP;
            Moody = entity.Moody;
            Fitch = entity.Fitch;

            return this;
        }

        public static CreditRatingDocument? FromEntity(CreditRating? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CreditRatingDocument().PopulateFromEntity(entity);
        }
    }
}