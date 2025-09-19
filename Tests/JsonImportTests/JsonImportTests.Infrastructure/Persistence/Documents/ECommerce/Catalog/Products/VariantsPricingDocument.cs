using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class VariantsPricingDocument : IVariantsPricingDocument
    {
        public decimal BasePrice { get; set; }
        public decimal CompareAtPrice { get; set; }
        public decimal CostPrice { get; set; }

        public VariantsPricing ToEntity(VariantsPricing? entity = default)
        {
            entity ??= new VariantsPricing();

            entity.BasePrice = BasePrice;
            entity.CompareAtPrice = CompareAtPrice;
            entity.CostPrice = CostPrice;

            return entity;
        }

        public VariantsPricingDocument PopulateFromEntity(VariantsPricing entity)
        {
            BasePrice = entity.BasePrice;
            CompareAtPrice = entity.CompareAtPrice;
            CostPrice = entity.CostPrice;

            return this;
        }

        public static VariantsPricingDocument? FromEntity(VariantsPricing? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VariantsPricingDocument().PopulateFromEntity(entity);
        }
    }
}