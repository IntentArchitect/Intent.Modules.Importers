using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class VariantDocument : IVariantDocument
    {
        public Guid Id { get; set; }
        public string SKU { get; set; } = default!;
        public string Name { get; set; } = default!;
        public VariantsPricingDocument Pricing { get; set; } = default!;
        IVariantsPricingDocument IVariantDocument.Pricing => Pricing;
        public VariantsInventoryDocument Inventory { get; set; } = default!;
        IVariantsInventoryDocument IVariantDocument.Inventory => Inventory;
        public List<VariantsImageDocument> Images { get; set; } = default!;
        IReadOnlyList<IVariantsImageDocument> IVariantDocument.Images => Images;
        public VariantsDimensionDocument Dimensions { get; set; } = default!;
        IVariantsDimensionDocument IVariantDocument.Dimensions => Dimensions;
        public VariantsAttributeDocument Attributes { get; set; } = default!;
        IVariantsAttributeDocument IVariantDocument.Attributes => Attributes;

        public Variant ToEntity(Variant? entity = default)
        {
            entity ??= new Variant();

            entity.Id = Id;
            entity.SKU = SKU ?? throw new Exception($"{nameof(entity.SKU)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Pricing = Pricing.ToEntity() ?? throw new Exception($"{nameof(entity.Pricing)} is null");
            entity.Inventory = Inventory.ToEntity() ?? throw new Exception($"{nameof(entity.Inventory)} is null");
            entity.Images = Images.Select(x => x.ToEntity()).ToList();
            entity.Dimensions = Dimensions.ToEntity() ?? throw new Exception($"{nameof(entity.Dimensions)} is null");
            entity.Attributes = Attributes.ToEntity() ?? throw new Exception($"{nameof(entity.Attributes)} is null");

            return entity;
        }

        public VariantDocument PopulateFromEntity(Variant entity)
        {
            Id = entity.Id;
            SKU = entity.SKU;
            Name = entity.Name;
            Pricing = VariantsPricingDocument.FromEntity(entity.Pricing)!;
            Inventory = VariantsInventoryDocument.FromEntity(entity.Inventory)!;
            Images = entity.Images.Select(x => VariantsImageDocument.FromEntity(x)!).ToList();
            Dimensions = VariantsDimensionDocument.FromEntity(entity.Dimensions)!;
            Attributes = VariantsAttributeDocument.FromEntity(entity.Attributes)!;

            return this;
        }

        public static VariantDocument? FromEntity(Variant? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VariantDocument().PopulateFromEntity(entity);
        }
    }
}