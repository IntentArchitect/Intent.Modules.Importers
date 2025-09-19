using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class TransactionPricingDocument : ITransactionPricingDocument
    {
        public decimal OrderPrice { get; set; }
        public decimal ExecutedPrice { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal MarketValue { get; set; }
        public decimal AccruedInterest { get; set; }

        public TransactionPricing ToEntity(TransactionPricing? entity = default)
        {
            entity ??= new TransactionPricing();

            entity.OrderPrice = OrderPrice;
            entity.ExecutedPrice = ExecutedPrice;
            entity.GrossAmount = GrossAmount;
            entity.NetAmount = NetAmount;
            entity.MarketValue = MarketValue;
            entity.AccruedInterest = AccruedInterest;

            return entity;
        }

        public TransactionPricingDocument PopulateFromEntity(TransactionPricing entity)
        {
            OrderPrice = entity.OrderPrice;
            ExecutedPrice = entity.ExecutedPrice;
            GrossAmount = entity.GrossAmount;
            NetAmount = entity.NetAmount;
            MarketValue = entity.MarketValue;
            AccruedInterest = entity.AccruedInterest;

            return this;
        }

        public static TransactionPricingDocument? FromEntity(TransactionPricing? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TransactionPricingDocument().PopulateFromEntity(entity);
        }
    }
}