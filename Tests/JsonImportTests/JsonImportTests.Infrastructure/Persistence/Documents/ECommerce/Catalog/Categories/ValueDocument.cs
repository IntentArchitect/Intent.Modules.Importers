using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class ValueDocument : IValueDocument
    {
        public string Id { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string Color { get; set; } = default!;
        public string Image { get; set; } = default!;

        public Value ToEntity(Value? entity = default)
        {
            entity ??= new Value();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Value = Value ?? throw new Exception($"{nameof(entity.Value)} is null");
            entity.DisplayName = DisplayName ?? throw new Exception($"{nameof(entity.DisplayName)} is null");
            entity.Color = Color ?? throw new Exception($"{nameof(entity.Color)} is null");
            entity.Image = Image ?? throw new Exception($"{nameof(entity.Image)} is null");

            return entity;
        }

        public ValueDocument PopulateFromEntity(Value entity)
        {
            Id = entity.Id;
            Value = entity.Value;
            DisplayName = entity.DisplayName;
            Color = entity.Color;
            Image = entity.Image;

            return this;
        }

        public static ValueDocument? FromEntity(Value? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ValueDocument().PopulateFromEntity(entity);
        }
    }
}