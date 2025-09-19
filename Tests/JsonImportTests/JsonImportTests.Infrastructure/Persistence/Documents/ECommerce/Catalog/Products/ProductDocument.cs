using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class ProductDocument : IProductDocument, ICosmosDBDocument<Product, ProductDocument>
    {
        [JsonProperty("_etag")]
        protected string? _etag;
        private string? _type;
        [JsonProperty("type")]
        string IItem.Type
        {
            get => _type ??= GetType().GetNameForDocument();
            set => _type = value;
        }
        string? IItemWithEtag.Etag => _etag;
        public string Id { get; set; }
        public string SKU { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string ShortDescription { get; set; } = default!;
        public string ProductType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public List<string> Tags { get; set; } = default!;
        IReadOnlyList<string> IProductDocument.Tags => Tags;
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string CreatedBy { get; set; } = default!;
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public bool IsFeatured { get; set; }
        public List<VariantDocument> Variants { get; set; } = default!;
        IReadOnlyList<IVariantDocument> IProductDocument.Variants => Variants;
        public ShippingDocument Shipping { get; set; } = default!;
        IShippingDocument IProductDocument.Shipping => Shipping;
        public ProductSEODocument SEO { get; set; } = default!;
        IProductSEODocument IProductDocument.SEO => SEO;
        public ReviewDocument Reviews { get; set; } = default!;
        IReviewDocument IProductDocument.Reviews => Reviews;
        public List<RelatedProductDocument> RelatedProducts { get; set; } = default!;
        IReadOnlyList<IRelatedProductDocument> IProductDocument.RelatedProducts => RelatedProducts;
        public ProductPricingDocument Pricing { get; set; } = default!;
        IProductPricingDocument IProductDocument.Pricing => Pricing;
        public List<ProductImageDocument> Images { get; set; } = default!;
        IReadOnlyList<IProductImageDocument> IProductDocument.Images => Images;
        public ProductCategoryDocument Category { get; set; } = default!;
        IProductCategoryDocument IProductDocument.Category => Category;
        public BrandDocument Brand { get; set; } = default!;
        IBrandDocument IProductDocument.Brand => Brand;
        public List<ProductAttributeDocument> Attributes { get; set; } = default!;
        IReadOnlyList<IProductAttributeDocument> IProductDocument.Attributes => Attributes;

        public Product ToEntity(Product? entity = default)
        {
            entity ??= new Product();

            entity.Id = Guid.Parse(Id);
            entity.SKU = SKU ?? throw new Exception($"{nameof(entity.SKU)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.ShortDescription = ShortDescription ?? throw new Exception($"{nameof(entity.ShortDescription)} is null");
            entity.ProductType = ProductType ?? throw new Exception($"{nameof(entity.ProductType)} is null");
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");
            entity.Tags = Tags ?? throw new Exception($"{nameof(entity.Tags)} is null");
            entity.CreatedDate = CreatedDate;
            entity.LastModified = LastModified;
            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.IsActive = IsActive;
            entity.IsVisible = IsVisible;
            entity.IsFeatured = IsFeatured;
            entity.Variants = Variants.Select(x => x.ToEntity()).ToList();
            entity.Shipping = Shipping.ToEntity() ?? throw new Exception($"{nameof(entity.Shipping)} is null");
            entity.SEO = SEO.ToEntity() ?? throw new Exception($"{nameof(entity.SEO)} is null");
            entity.Reviews = Reviews.ToEntity() ?? throw new Exception($"{nameof(entity.Reviews)} is null");
            entity.RelatedProducts = RelatedProducts.Select(x => x.ToEntity()).ToList();
            entity.Pricing = Pricing.ToEntity() ?? throw new Exception($"{nameof(entity.Pricing)} is null");
            entity.Images = Images.Select(x => x.ToEntity()).ToList();
            entity.Category = Category.ToEntity() ?? throw new Exception($"{nameof(entity.Category)} is null");
            entity.Brand = Brand.ToEntity() ?? throw new Exception($"{nameof(entity.Brand)} is null");
            entity.Attributes = Attributes.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ProductDocument PopulateFromEntity(Product entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            SKU = entity.SKU;
            Name = entity.Name;
            Description = entity.Description;
            ShortDescription = entity.ShortDescription;
            ProductType = entity.ProductType;
            Status = entity.Status;
            Tags = entity.Tags.ToList();
            CreatedDate = entity.CreatedDate;
            LastModified = entity.LastModified;
            CreatedBy = entity.CreatedBy;
            IsActive = entity.IsActive;
            IsVisible = entity.IsVisible;
            IsFeatured = entity.IsFeatured;
            Variants = entity.Variants.Select(x => VariantDocument.FromEntity(x)!).ToList();
            Shipping = ShippingDocument.FromEntity(entity.Shipping)!;
            SEO = ProductSEODocument.FromEntity(entity.SEO)!;
            Reviews = ReviewDocument.FromEntity(entity.Reviews)!;
            RelatedProducts = entity.RelatedProducts.Select(x => RelatedProductDocument.FromEntity(x)!).ToList();
            Pricing = ProductPricingDocument.FromEntity(entity.Pricing)!;
            Images = entity.Images.Select(x => ProductImageDocument.FromEntity(x)!).ToList();
            Category = ProductCategoryDocument.FromEntity(entity.Category)!;
            Brand = BrandDocument.FromEntity(entity.Brand)!;
            Attributes = entity.Attributes.Select(x => ProductAttributeDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static ProductDocument? FromEntity(Product? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProductDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}