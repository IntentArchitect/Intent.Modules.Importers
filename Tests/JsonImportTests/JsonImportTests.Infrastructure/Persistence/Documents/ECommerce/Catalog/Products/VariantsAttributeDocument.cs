using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class VariantsAttributeDocument : IVariantsAttributeDocument
    {
        public string Color { get; set; } = default!;
        public string Size { get; set; } = default!;
        public string Material { get; set; } = default!;
        public decimal Weight { get; set; }

        public VariantsAttribute ToEntity(VariantsAttribute? entity = default)
        {
            entity ??= new VariantsAttribute();

            entity.Color = Color ?? throw new Exception($"{nameof(entity.Color)} is null");
            entity.Size = Size ?? throw new Exception($"{nameof(entity.Size)} is null");
            entity.Material = Material ?? throw new Exception($"{nameof(entity.Material)} is null");
            entity.Weight = Weight;

            return entity;
        }

        public VariantsAttributeDocument PopulateFromEntity(VariantsAttribute entity)
        {
            Color = entity.Color;
            Size = entity.Size;
            Material = entity.Material;
            Weight = entity.Weight;

            return this;
        }

        public static VariantsAttributeDocument? FromEntity(VariantsAttribute? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VariantsAttributeDocument().PopulateFromEntity(entity);
        }
    }
}