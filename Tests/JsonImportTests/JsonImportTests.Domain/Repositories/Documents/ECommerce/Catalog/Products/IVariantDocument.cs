using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IVariantDocument
    {
        Guid Id { get; }
        string SKU { get; }
        string Name { get; }
        IVariantsPricingDocument Pricing { get; }
        IVariantsInventoryDocument Inventory { get; }
        IReadOnlyList<IVariantsImageDocument> Images { get; }
        IVariantsDimensionDocument Dimensions { get; }
        IVariantsAttributeDocument Attributes { get; }
    }
}