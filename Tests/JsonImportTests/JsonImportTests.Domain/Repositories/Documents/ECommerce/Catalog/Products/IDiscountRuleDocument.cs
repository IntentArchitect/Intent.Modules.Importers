using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products
{
    public interface IDiscountRuleDocument
    {
        string Id { get; }
        Guid RuleId { get; }
        string Type { get; }
        decimal Value { get; }
        decimal MinQuantity { get; }
        DateTime StartDate { get; }
        DateTime EndDate { get; }
    }
}