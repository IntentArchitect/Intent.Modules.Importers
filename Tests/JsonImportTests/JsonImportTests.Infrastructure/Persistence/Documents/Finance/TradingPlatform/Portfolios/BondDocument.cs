using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class BondDocument : IBondDocument
    {
        public decimal Percentage { get; set; }
        public decimal Value { get; set; }
        public List<TypeDocument> Types { get; set; } = default!;
        IReadOnlyList<ITypeDocument> IBondDocument.Types => Types;

        public Bond ToEntity(Bond? entity = default)
        {
            entity ??= new Bond();

            entity.Percentage = Percentage;
            entity.Value = Value;
            entity.Types = Types.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public BondDocument PopulateFromEntity(Bond entity)
        {
            Percentage = entity.Percentage;
            Value = entity.Value;
            Types = entity.Types.Select(x => TypeDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static BondDocument? FromEntity(Bond? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new BondDocument().PopulateFromEntity(entity);
        }
    }
}