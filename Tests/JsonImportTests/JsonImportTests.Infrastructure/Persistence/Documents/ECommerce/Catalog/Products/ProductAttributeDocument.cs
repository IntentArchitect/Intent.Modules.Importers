using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ProductAttributeDocument : IProductAttributeDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;
        public string Type { get; set; } = default!;
        public bool IsFilterable { get; set; }
        public bool IsSearchable { get; set; }
        public decimal DisplayOrder { get; set; }

        public ProductAttribute ToEntity(ProductAttribute? entity = default)
        {
            entity ??= new ProductAttribute();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Value = Value ?? throw new Exception($"{nameof(entity.Value)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.IsFilterable = IsFilterable;
            entity.IsSearchable = IsSearchable;
            entity.DisplayOrder = DisplayOrder;

            return entity;
        }

        public ProductAttributeDocument PopulateFromEntity(ProductAttribute entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Value = entity.Value;
            Type = entity.Type;
            IsFilterable = entity.IsFilterable;
            IsSearchable = entity.IsSearchable;
            DisplayOrder = entity.DisplayOrder;

            return this;
        }

        public static ProductAttributeDocument? FromEntity(ProductAttribute? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductAttributeDocument().PopulateFromEntity(entity);
        }
    }
}