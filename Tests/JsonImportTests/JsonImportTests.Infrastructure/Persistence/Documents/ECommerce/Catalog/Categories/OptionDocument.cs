using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class OptionDocument : IOptionDocument
    {
        public string Id { get; set; } = default!;
        public string Label { get; set; } = default!;
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }

        public Option ToEntity(Option? entity = default)
        {
            entity ??= new Option();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Label = Label ?? throw new Exception($"{nameof(entity.Label)} is null");
            entity.MinValue = MinValue;
            entity.MaxValue = MaxValue;

            return entity;
        }

        public OptionDocument PopulateFromEntity(Option entity)
        {
            Id = entity.Id;
            Label = entity.Label;
            MinValue = entity.MinValue;
            MaxValue = entity.MaxValue;

            return this;
        }

        public static OptionDocument? FromEntity(Option? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OptionDocument().PopulateFromEntity(entity);
        }
    }
}