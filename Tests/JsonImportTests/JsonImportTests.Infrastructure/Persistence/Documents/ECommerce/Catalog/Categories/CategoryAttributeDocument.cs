using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class CategoryAttributeDocument : ICategoryAttributeDocument
    {
        public string Id { get; set; } = default!;
        public Guid AttributeId { get; set; }
        public string Name { get; set; } = default!;
        public string Type { get; set; } = default!;
        public bool IsRequired { get; set; }
        public bool IsFilterable { get; set; }
        public List<ValueDocument> Values { get; set; } = default!;
        IReadOnlyList<IValueDocument> ICategoryAttributeDocument.Values => Values;

        public CategoryAttribute ToEntity(CategoryAttribute? entity = default)
        {
            entity ??= new CategoryAttribute();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.AttributeId = AttributeId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.IsRequired = IsRequired;
            entity.IsFilterable = IsFilterable;
            entity.Values = Values.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public CategoryAttributeDocument PopulateFromEntity(CategoryAttribute entity)
        {
            Id = entity.Id;
            AttributeId = entity.AttributeId;
            Name = entity.Name;
            Type = entity.Type;
            IsRequired = entity.IsRequired;
            IsFilterable = entity.IsFilterable;
            Values = entity.Values.Select(x => ValueDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static CategoryAttributeDocument? FromEntity(CategoryAttribute? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CategoryAttributeDocument().PopulateFromEntity(entity);
        }
    }
}