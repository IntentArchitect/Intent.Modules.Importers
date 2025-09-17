using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities;
using JsonImportTests.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents
{
    internal class OrderDocument : IOrderDocument
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }

        public Order ToEntity(Order? entity = default)
        {
            entity ??= new Order();

            entity.Id = Id;
            entity.Amount = Amount;

            return entity;
        }

        public OrderDocument PopulateFromEntity(Order entity)
        {
            Id = entity.Id;
            Amount = entity.Amount;

            return this;
        }

        public static OrderDocument? FromEntity(Order? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrderDocument().PopulateFromEntity(entity);
        }
    }
}