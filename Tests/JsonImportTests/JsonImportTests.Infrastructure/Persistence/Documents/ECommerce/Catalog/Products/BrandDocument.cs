using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class BrandDocument : IBrandDocument
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string LogoUrl { get; set; } = default!;
        public string Website { get; set; } = default!;

        public Brand ToEntity(Brand? entity = default)
        {
            entity ??= new Brand();

            entity.Id = Id;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.LogoUrl = LogoUrl ?? throw new Exception($"{nameof(entity.LogoUrl)} is null");
            entity.Website = Website ?? throw new Exception($"{nameof(entity.Website)} is null");

            return entity;
        }

        public BrandDocument PopulateFromEntity(Brand entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            LogoUrl = entity.LogoUrl;
            Website = entity.Website;

            return this;
        }

        public static BrandDocument? FromEntity(Brand? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new BrandDocument().PopulateFromEntity(entity);
        }
    }
}