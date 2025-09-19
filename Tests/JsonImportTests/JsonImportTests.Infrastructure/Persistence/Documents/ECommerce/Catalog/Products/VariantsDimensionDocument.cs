using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class VariantsDimensionDocument : IVariantsDimensionDocument
    {
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string Unit { get; set; } = default!;

        public VariantsDimension ToEntity(VariantsDimension? entity = default)
        {
            entity ??= new VariantsDimension();

            entity.Length = Length;
            entity.Width = Width;
            entity.Height = Height;
            entity.Weight = Weight;
            entity.Unit = Unit ?? throw new Exception($"{nameof(entity.Unit)} is null");

            return entity;
        }

        public VariantsDimensionDocument PopulateFromEntity(VariantsDimension entity)
        {
            Length = entity.Length;
            Width = entity.Width;
            Height = entity.Height;
            Weight = entity.Weight;
            Unit = entity.Unit;

            return this;
        }

        public static VariantsDimensionDocument? FromEntity(VariantsDimension? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VariantsDimensionDocument().PopulateFromEntity(entity);
        }
    }
}