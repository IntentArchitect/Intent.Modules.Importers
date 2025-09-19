using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities;
using JsonImportTests.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents
{
    internal class ItemDocument : IItemDocument
    {
        public string Id { get; set; } = default!;
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }

        public Item ToEntity(Item? entity = default)
        {
            entity ??= new Item();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ProductId = ProductId;
            entity.Quantity = Quantity;
            entity.Price = Price;

            return entity;
        }

        public ItemDocument PopulateFromEntity(Item entity)
        {
            Id = entity.Id;
            ProductId = entity.ProductId;
            Quantity = entity.Quantity;
            Price = entity.Price;

            return this;
        }

        public static ItemDocument? FromEntity(Item? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ItemDocument().PopulateFromEntity(entity);
        }
    }
}