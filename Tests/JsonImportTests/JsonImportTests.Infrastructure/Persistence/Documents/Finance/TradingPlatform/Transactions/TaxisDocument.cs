using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class TaxisDocument : ITaxisDocument
    {
        public decimal TransactionTax { get; set; }
        public decimal StampDuty { get; set; }
        public decimal WithholdingTax { get; set; }
        public decimal TotalTaxes { get; set; }
        public string TaxCurrency { get; set; } = default!;

        public Taxis ToEntity(Taxis? entity = default)
        {
            entity ??= new Taxis();

            entity.TransactionTax = TransactionTax;
            entity.StampDuty = StampDuty;
            entity.WithholdingTax = WithholdingTax;
            entity.TotalTaxes = TotalTaxes;
            entity.TaxCurrency = TaxCurrency ?? throw new Exception($"{nameof(entity.TaxCurrency)} is null");

            return entity;
        }

        public TaxisDocument PopulateFromEntity(Taxis entity)
        {
            TransactionTax = entity.TransactionTax;
            StampDuty = entity.StampDuty;
            WithholdingTax = entity.WithholdingTax;
            TotalTaxes = entity.TotalTaxes;
            TaxCurrency = entity.TaxCurrency;

            return this;
        }

        public static TaxisDocument? FromEntity(Taxis? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TaxisDocument().PopulateFromEntity(entity);
        }
    }
}