using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ProductPricingDocument : IProductPricingDocument
    {
        public decimal BasePrice { get; set; }
        public decimal CompareAtPrice { get; set; }
        public decimal CostPrice { get; set; }
        public string Currency { get; set; } = default!;
        public string TaxClass { get; set; } = default!;
        public List<DiscountRuleDocument> DiscountRules { get; set; } = default!;
        IReadOnlyList<IDiscountRuleDocument> IProductPricingDocument.DiscountRules => DiscountRules;

        public ProductPricing ToEntity(ProductPricing? entity = default)
        {
            entity ??= new ProductPricing();

            entity.BasePrice = BasePrice;
            entity.CompareAtPrice = CompareAtPrice;
            entity.CostPrice = CostPrice;
            entity.Currency = Currency ?? throw new Exception($"{nameof(entity.Currency)} is null");
            entity.TaxClass = TaxClass ?? throw new Exception($"{nameof(entity.TaxClass)} is null");
            entity.DiscountRules = DiscountRules.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ProductPricingDocument PopulateFromEntity(ProductPricing entity)
        {
            BasePrice = entity.BasePrice;
            CompareAtPrice = entity.CompareAtPrice;
            CostPrice = entity.CostPrice;
            Currency = entity.Currency;
            TaxClass = entity.TaxClass;
            DiscountRules = entity.DiscountRules.Select(x => DiscountRuleDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ProductPricingDocument? FromEntity(ProductPricing? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductPricingDocument().PopulateFromEntity(entity);
        }
    }
}