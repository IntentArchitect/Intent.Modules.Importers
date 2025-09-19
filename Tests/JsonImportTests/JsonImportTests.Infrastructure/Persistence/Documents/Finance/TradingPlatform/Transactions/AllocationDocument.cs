using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class AllocationDocument : IAllocationDocument
    {
        public string Id { get; set; } = default!;
        public Guid AccountId { get; set; }
        public decimal AllocationPercentage { get; set; }
        public decimal AllocatedQuantity { get; set; }
        public decimal AllocatedAmount { get; set; }

        public Allocation ToEntity(Allocation? entity = default)
        {
            entity ??= new Allocation();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.AccountId = AccountId;
            entity.AllocationPercentage = AllocationPercentage;
            entity.AllocatedQuantity = AllocatedQuantity;
            entity.AllocatedAmount = AllocatedAmount;

            return entity;
        }

        public AllocationDocument PopulateFromEntity(Allocation entity)
        {
            Id = entity.Id;
            AccountId = entity.AccountId;
            AllocationPercentage = entity.AllocationPercentage;
            AllocatedQuantity = entity.AllocatedQuantity;
            AllocatedAmount = entity.AllocatedAmount;

            return this;
        }

        public static AllocationDocument? FromEntity(Allocation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AllocationDocument().PopulateFromEntity(entity);
        }
    }
}