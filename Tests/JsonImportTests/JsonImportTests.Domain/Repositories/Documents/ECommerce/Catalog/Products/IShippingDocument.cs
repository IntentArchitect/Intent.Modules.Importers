using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IShippingDocument
    {
        decimal Weight { get; }
        string ShippingClass { get; }
        bool RequiresShipping { get; }
        bool IsFreeShipping { get; }
        decimal HandlingTime { get; }
        IShippingDimensionDocument Dimensions { get; }
    }
}