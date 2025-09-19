using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class CounterpartyDocument : ICounterpartyDocument
    {
        public string BrokerId { get; set; } = default!;
        public string BrokerName { get; set; } = default!;
        public string DealerId { get; set; } = default!;
        public string DealerName { get; set; } = default!;
        public string PrimeBroker { get; set; } = default!;

        public Counterparty ToEntity(Counterparty? entity = default)
        {
            entity ??= new Counterparty();

            entity.BrokerId = BrokerId ?? throw new Exception($"{nameof(entity.BrokerId)} is null");
            entity.BrokerName = BrokerName ?? throw new Exception($"{nameof(entity.BrokerName)} is null");
            entity.DealerId = DealerId ?? throw new Exception($"{nameof(entity.DealerId)} is null");
            entity.DealerName = DealerName ?? throw new Exception($"{nameof(entity.DealerName)} is null");
            entity.PrimeBroker = PrimeBroker ?? throw new Exception($"{nameof(entity.PrimeBroker)} is null");

            return entity;
        }

        public CounterpartyDocument PopulateFromEntity(Counterparty entity)
        {
            BrokerId = entity.BrokerId;
            BrokerName = entity.BrokerName;
            DealerId = entity.DealerId;
            DealerName = entity.DealerName;
            PrimeBroker = entity.PrimeBroker;

            return this;
        }

        public static CounterpartyDocument? FromEntity(Counterparty? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CounterpartyDocument().PopulateFromEntity(entity);
        }
    }
}