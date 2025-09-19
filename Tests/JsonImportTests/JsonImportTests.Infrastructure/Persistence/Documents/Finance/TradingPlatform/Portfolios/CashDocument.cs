using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class CashDocument : ICashDocument
    {
        public decimal Percentage { get; set; }
        public decimal Value { get; set; }

        public Cash ToEntity(Cash? entity = default)
        {
            entity ??= new Cash();

            entity.Percentage = Percentage;
            entity.Value = Value;

            return entity;
        }

        public CashDocument PopulateFromEntity(Cash entity)
        {
            Percentage = entity.Percentage;
            Value = entity.Value;

            return this;
        }

        public static CashDocument? FromEntity(Cash? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CashDocument().PopulateFromEntity(entity);
        }
    }
}