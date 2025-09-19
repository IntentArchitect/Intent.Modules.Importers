using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Categories
{
    public interface IcategoryCategoryDocument
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        string Slug { get; }
        Guid ParentCategoryId { get; }
        decimal Level { get; }
        string Path { get; }
        decimal DisplayOrder { get; }
        bool IsActive { get; }
        bool IsVisible { get; }
        decimal ProductCount { get; }
        DateTime CreatedDate { get; }
        DateTime LastModified { get; }
        string CreatedBy { get; }
        IReadOnlyList<ISubCategoryDocument> SubCategories { get; }
        ICategoryMetadataDocument Metadata { get; }
        IReadOnlyList<IFilterDocument> Filters { get; }
        IReadOnlyList<IFeaturedProductDocument> FeaturedProducts { get; }
        IReadOnlyList<ICategoryAttributeDocument> Attributes { get; }
    }
}