using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.ECommerce.Catalog.Products;
using JsonImportTests.Domain.Repositories.Documents.ECommerce.Catalog.Products;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.ECommerce.Catalog.Products
{
    internal class DiscountRuleDocument : IDiscountRuleDocument
    {
        public string Id { get; set; } = default!;
        public Guid RuleId { get; set; }
        public string Type { get; set; } = default!;
        public decimal Value { get; set; }
        public decimal MinQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DiscountRule ToEntity(DiscountRule? entity = default)
        {
            entity ??= new DiscountRule();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.RuleId = RuleId;
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Value = Value;
            entity.MinQuantity = MinQuantity;
            entity.StartDate = StartDate;
            entity.EndDate = EndDate;

            return entity;
        }

        public DiscountRuleDocument PopulateFromEntity(DiscountRule entity)
        {
            Id = entity.Id;
            RuleId = entity.RuleId;
            Type = entity.Type;
            Value = entity.Value;
            MinQuantity = entity.MinQuantity;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;

            return this;
        }

        public static DiscountRuleDocument? FromEntity(DiscountRule? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DiscountRuleDocument().PopulateFromEntity(entity);
        }
    }
}