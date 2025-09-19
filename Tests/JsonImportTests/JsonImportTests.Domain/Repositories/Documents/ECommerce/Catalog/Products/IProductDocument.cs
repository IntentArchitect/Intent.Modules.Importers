using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IProductDocument
    {
        string Id { get; }
        string SKU { get; }
        string Name { get; }
        string Description { get; }
        string ShortDescription { get; }
        string ProductType { get; }
        string Status { get; }
        IReadOnlyList<string> Tags { get; }
        DateTime CreatedDate { get; }
        DateTime LastModified { get; }
        string CreatedBy { get; }
        bool IsActive { get; }
        bool IsVisible { get; }
        bool IsFeatured { get; }
        IReadOnlyList<IVariantDocument> Variants { get; }
        IShippingDocument Shipping { get; }
        IProductSEODocument SEO { get; }
        IReviewDocument Reviews { get; }
        IReadOnlyList<IRelatedProductDocument> RelatedProducts { get; }
        IProductPricingDocument Pricing { get; }
        IReadOnlyList<IProductImageDocument> Images { get; }
        IProductCategoryDocument Category { get; }
        IBrandDocument Brand { get; }
        IReadOnlyList<IProductAttributeDocument> Attributes { get; }
    }
}