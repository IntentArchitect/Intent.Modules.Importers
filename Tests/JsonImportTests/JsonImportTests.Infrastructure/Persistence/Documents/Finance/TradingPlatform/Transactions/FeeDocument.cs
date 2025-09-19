using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class FeeDocument : IFeeDocument
    {
        public decimal Commission { get; set; }
        public decimal RegulatoryFees { get; set; }
        public decimal ExchangeFees { get; set; }
        public decimal ClearingFees { get; set; }
        public decimal OtherFees { get; set; }
        public decimal TotalFees { get; set; }
        public string Currency { get; set; } = default!;

        public Fee ToEntity(Fee? entity = default)
        {
            entity ??= new Fee();

            entity.Commission = Commission;
            entity.RegulatoryFees = RegulatoryFees;
            entity.ExchangeFees = ExchangeFees;
            entity.ClearingFees = ClearingFees;
            entity.OtherFees = OtherFees;
            entity.TotalFees = TotalFees;
            entity.Currency = Currency ?? throw new Exception($"{nameof(entity.Currency)} is null");

            return entity;
        }

        public FeeDocument PopulateFromEntity(Fee entity)
        {
            Commission = entity.Commission;
            RegulatoryFees = entity.RegulatoryFees;
            ExchangeFees = entity.ExchangeFees;
            ClearingFees = entity.ClearingFees;
            OtherFees = entity.OtherFees;
            TotalFees = entity.TotalFees;
            Currency = entity.Currency;

            return this;
        }

        public static FeeDocument? FromEntity(Fee? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new FeeDocument().PopulateFromEntity(entity);
        }
    }
}