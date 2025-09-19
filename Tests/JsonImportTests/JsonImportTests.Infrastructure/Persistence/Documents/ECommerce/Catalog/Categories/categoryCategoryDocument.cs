using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Categories;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories;
using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Categories
{
    internal class categoryCategoryDocument : IcategoryCategoryDocument, ICosmosDBDocument<categoryCategory, categoryCategoryDocument>
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
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public Guid ParentCategoryId { get; set; }
        public decimal Level { get; set; }
        public string Path { get; set; } = default!;
        public decimal DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsVisible { get; set; }
        public decimal ProductCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string CreatedBy { get; set; } = default!;
        public List<SubCategoryDocument> SubCategories { get; set; } = default!;
        IReadOnlyList<ISubCategoryDocument> IcategoryCategoryDocument.SubCategories => SubCategories;
        public CategoryMetadataDocument Metadata { get; set; } = default!;
        ICategoryMetadataDocument IcategoryCategoryDocument.Metadata => Metadata;
        public List<FilterDocument> Filters { get; set; } = default!;
        IReadOnlyList<IFilterDocument> IcategoryCategoryDocument.Filters => Filters;
        public List<FeaturedProductDocument> FeaturedProducts { get; set; } = default!;
        IReadOnlyList<IFeaturedProductDocument> IcategoryCategoryDocument.FeaturedProducts => FeaturedProducts;
        public List<CategoryAttributeDocument> Attributes { get; set; } = default!;
        IReadOnlyList<ICategoryAttributeDocument> IcategoryCategoryDocument.Attributes => Attributes;

        public categoryCategory ToEntity(categoryCategory? entity = default)
        {
            entity ??= new categoryCategory();

            entity.Id = Guid.Parse(Id);
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Slug = Slug ?? throw new Exception($"{nameof(entity.Slug)} is null");
            entity.ParentCategoryId = ParentCategoryId;
            entity.Level = Level;
            entity.Path = Path ?? throw new Exception($"{nameof(entity.Path)} is null");
            entity.DisplayOrder = DisplayOrder;
            entity.IsActive = IsActive;
            entity.IsVisible = IsVisible;
            entity.ProductCount = ProductCount;
            entity.CreatedDate = CreatedDate;
            entity.LastModified = LastModified;
            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.SubCategories = SubCategories.Select(x => x.ToEntity()).ToList();
            entity.Metadata = Metadata.ToEntity() ?? throw new Exception($"{nameof(entity.Metadata)} is null");
            entity.Filters = Filters.Select(x => x.ToEntity()).ToList();
            entity.FeaturedProducts = FeaturedProducts.Select(x => x.ToEntity()).ToList();
            entity.Attributes = Attributes.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public categoryCategoryDocument PopulateFromEntity(categoryCategory entity, Func<string, string?> getEtag)
        {
            Id = entity.Id.ToString();
            Name = entity.Name;
            Description = entity.Description;
            Slug = entity.Slug;
            ParentCategoryId = entity.ParentCategoryId;
            Level = entity.Level;
            Path = entity.Path;
            DisplayOrder = entity.DisplayOrder;
            IsActive = entity.IsActive;
            IsVisible = entity.IsVisible;
            ProductCount = entity.ProductCount;
            CreatedDate = entity.CreatedDate;
            LastModified = entity.LastModified;
            CreatedBy = entity.CreatedBy;
            SubCategories = entity.SubCategories.Select(x => SubCategoryDocument.FromEntity(x)!).ToList();
            Metadata = CategoryMetadataDocument.FromEntity(entity.Metadata)!;
            Filters = entity.Filters.Select(x => FilterDocument.FromEntity(x)!).ToList();
            FeaturedProducts = entity.FeaturedProducts.Select(x => FeaturedProductDocument.FromEntity(x)!).ToList();
            Attributes = entity.Attributes.Select(x => CategoryAttributeDocument.FromEntity(x)!).ToList();

            _etag = _etag == null ? getEtag(((IItem)this).Id) : _etag;

            return this;
        }

        public static categoryCategoryDocument? FromEntity(categoryCategory? entity, Func<string, string?> getEtag)
        {
            if (entity is null)
            {
                return null;
            }

            return new categoryCategoryDocument().PopulateFromEntity(entity, getEtag);
        }
    }
}