using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class SettlementDetailDocument : ISettlementDetailDocument
    {
        public string SettlementCurrency { get; set; } = default!;
        public decimal ExchangeRate { get; set; }
        public decimal SettlementAmount { get; set; }
        public string SettlementMethod { get; set; } = default!;
        public string CustodianReference { get; set; } = default!;
        public string ClearingHouse { get; set; } = default!;

        public SettlementDetail ToEntity(SettlementDetail? entity = default)
        {
            entity ??= new SettlementDetail();

            entity.SettlementCurrency = SettlementCurrency ?? throw new Exception($"{nameof(entity.SettlementCurrency)} is null");
            entity.ExchangeRate = ExchangeRate;
            entity.SettlementAmount = SettlementAmount;
            entity.SettlementMethod = SettlementMethod ?? throw new Exception($"{nameof(entity.SettlementMethod)} is null");
            entity.CustodianReference = CustodianReference ?? throw new Exception($"{nameof(entity.CustodianReference)} is null");
            entity.ClearingHouse = ClearingHouse ?? throw new Exception($"{nameof(entity.ClearingHouse)} is null");

            return entity;
        }

        public SettlementDetailDocument PopulateFromEntity(SettlementDetail entity)
        {
            SettlementCurrency = entity.SettlementCurrency;
            ExchangeRate = entity.ExchangeRate;
            SettlementAmount = entity.SettlementAmount;
            SettlementMethod = entity.SettlementMethod;
            CustodianReference = entity.CustodianReference;
            ClearingHouse = entity.ClearingHouse;

            return this;
        }

        public static SettlementDetailDocument? FromEntity(SettlementDetail? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SettlementDetailDocument().PopulateFromEntity(entity);
        }
    }
}