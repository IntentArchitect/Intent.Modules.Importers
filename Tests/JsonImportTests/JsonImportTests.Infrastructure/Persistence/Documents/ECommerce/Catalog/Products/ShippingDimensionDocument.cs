using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ShippingDimensionDocument : IShippingDimensionDocument
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string Unit { get; set; } = default!;

        public ShippingDimension ToEntity(ShippingDimension? entity = default)
        {
            entity ??= new ShippingDimension();

            entity.Length = Length;
            entity.Width = Width;
            entity.Height = Height;
            entity.Unit = Unit ?? throw new Exception($"{nameof(entity.Unit)} is null");

            return entity;
        }

        public ShippingDimensionDocument PopulateFromEntity(ShippingDimension entity)
        {
            Length = entity.Length;
            Width = entity.Width;
            Height = entity.Height;
            Unit = entity.Unit;

            return this;
        }

        public static ShippingDimensionDocument? FromEntity(ShippingDimension? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ShippingDimensionDocument().PopulateFromEntity(entity);
        }
    }
}