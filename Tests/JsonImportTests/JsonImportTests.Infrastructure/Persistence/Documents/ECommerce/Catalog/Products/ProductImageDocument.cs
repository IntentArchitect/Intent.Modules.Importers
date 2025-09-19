using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ProductImageDocument : IProductImageDocument
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = default!;
        public string AltText { get; set; } = default!;
        public decimal Position { get; set; }
        public bool IsPrimary { get; set; }
        public string ImageType { get; set; } = default!;

        public ProductImage ToEntity(ProductImage? entity = default)
        {
            entity ??= new ProductImage();

            entity.Id = Id;
            entity.Url = Url ?? throw new Exception($"{nameof(entity.Url)} is null");
            entity.AltText = AltText ?? throw new Exception($"{nameof(entity.AltText)} is null");
            entity.Position = Position;
            entity.IsPrimary = IsPrimary;
            entity.ImageType = ImageType ?? throw new Exception($"{nameof(entity.ImageType)} is null");

            return entity;
        }

        public ProductImageDocument PopulateFromEntity(ProductImage entity)
        {
            Id = entity.Id;
            Url = entity.Url;
            AltText = entity.AltText;
            Position = entity.Position;
            IsPrimary = entity.IsPrimary;
            ImageType = entity.ImageType;

            return this;
        }

        public static ProductImageDocument? FromEntity(ProductImage? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductImageDocument().PopulateFromEntity(entity);
        }
    }
}