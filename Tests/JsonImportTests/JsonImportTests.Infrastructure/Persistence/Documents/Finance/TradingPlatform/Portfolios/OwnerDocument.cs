using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Portfolios;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Portfolios;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Portfolios
{
    internal class OwnerDocument : IOwnerDocument
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Phone { get; set; } = default!;

        public Owner ToEntity(Owner? entity = default)
        {
            entity ??= new Owner();

            entity.CustomerId = CustomerId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.Phone = Phone ?? throw new Exception($"{nameof(entity.Phone)} is null");

            return entity;
        }

        public OwnerDocument PopulateFromEntity(Owner entity)
        {
            CustomerId = entity.CustomerId;
            Name = entity.Name;
            Email = entity.Email;
            Phone = entity.Phone;

            return this;
        }

        public static OwnerDocument? FromEntity(Owner? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OwnerDocument().PopulateFromEntity(entity);
        }
    }
}