using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class OtherDocument : IOtherDocument
    {
        public decimal Percentage { get; set; }
        public decimal Value { get; set; }

        public Other ToEntity(Other? entity = default)
        {
            entity ??= new Other();

            entity.Percentage = Percentage;
            entity.Value = Value;

            return entity;
        }

        public OtherDocument PopulateFromEntity(Other entity)
        {
            Percentage = entity.Percentage;
            Value = entity.Value;

            return this;
        }

        public static OtherDocument? FromEntity(Other? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OtherDocument().PopulateFromEntity(entity);
        }
    }
}