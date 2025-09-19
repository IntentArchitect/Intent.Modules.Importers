using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ShippingDocument : IShippingDocument
    {
        public decimal Weight { get; set; }
        public string ShippingClass { get; set; } = default!;
        public bool RequiresShipping { get; set; }
        public bool IsFreeShipping { get; set; }
        public decimal HandlingTime { get; set; }
        public ShippingDimensionDocument Dimensions { get; set; } = default!;
        IShippingDimensionDocument IShippingDocument.Dimensions => Dimensions;

        public Shipping ToEntity(Shipping? entity = default)
        {
            entity ??= new Shipping();

            entity.Weight = Weight;
            entity.ShippingClass = ShippingClass ?? throw new Exception($"{nameof(entity.ShippingClass)} is null");
            entity.RequiresShipping = RequiresShipping;
            entity.IsFreeShipping = IsFreeShipping;
            entity.HandlingTime = HandlingTime;
            entity.Dimensions = Dimensions.ToEntity() ?? throw new Exception($"{nameof(entity.Dimensions)} is null");

            return entity;
        }

        public ShippingDocument PopulateFromEntity(Shipping entity)
        {
            Weight = entity.Weight;
            ShippingClass = entity.ShippingClass;
            RequiresShipping = entity.RequiresShipping;
            IsFreeShipping = entity.IsFreeShipping;
            HandlingTime = entity.HandlingTime;
            Dimensions = ShippingDimensionDocument.FromEntity(entity.Dimensions)!;

            return this;
        }

        public static ShippingDocument? FromEntity(Shipping? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ShippingDocument().PopulateFromEntity(entity);
        }
    }
}