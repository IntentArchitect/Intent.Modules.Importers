using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class SubCategoryDocument : ISubCategoryDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public decimal ProductCount { get; set; }
        public bool IsActive { get; set; }

        public SubCategory ToEntity(SubCategory? entity = default)
        {
            entity ??= new SubCategory();

            entity.Id = Id;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Slug = Slug ?? throw new Exception($"{nameof(entity.Slug)} is null");
            entity.ProductCount = ProductCount;
            entity.IsActive = IsActive;

            return entity;
        }

        public SubCategoryDocument PopulateFromEntity(SubCategory entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Slug = entity.Slug;
            ProductCount = entity.ProductCount;
            IsActive = entity.IsActive;

            return this;
        }

        public static SubCategoryDocument? FromEntity(SubCategory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SubCategoryDocument().PopulateFromEntity(entity);
        }
    }
}