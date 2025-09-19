using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class TypeDocument : ITypeDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Percentage { get; set; }
        public decimal Value { get; set; }

        public Domain.Entities.Finance.TradingPlatform.Portfolios.Type ToEntity(Domain.Entities.Finance.TradingPlatform.Portfolios.Type? entity = default)
        {
            entity ??= new Domain.Entities.Finance.TradingPlatform.Portfolios.Type();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Percentage = Percentage;
            entity.Value = Value;

            return entity;
        }

        public TypeDocument PopulateFromEntity(Domain.Entities.Finance.TradingPlatform.Portfolios.Type entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Percentage = entity.Percentage;
            Value = entity.Value;

            return this;
        }

        public static TypeDocument? FromEntity(Domain.Entities.Finance.TradingPlatform.Portfolios.Type? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TypeDocument().PopulateFromEntity(entity);
        }
    }
}