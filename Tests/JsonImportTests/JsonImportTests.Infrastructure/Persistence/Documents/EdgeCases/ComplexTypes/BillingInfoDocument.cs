using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class BillingInfoDocument : IBillingInfoDocument
    {
        public string Plan { get; set; } = default!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = default!;
        public DateTime NextBillingDate { get; set; }
        public PaymentMethodDocument PaymentMethod { get; set; } = default!;
        IPaymentMethodDocument IBillingInfoDocument.PaymentMethod => PaymentMethod;

        public BillingInfo ToEntity(BillingInfo? entity = default)
        {
            entity ??= new BillingInfo();

            entity.Plan = Plan ?? throw new Exception($"{nameof(entity.Plan)} is null");
            entity.Amount = Amount;
            entity.Currency = Currency ?? throw new Exception($"{nameof(entity.Currency)} is null");
            entity.NextBillingDate = NextBillingDate;
            entity.PaymentMethod = PaymentMethod.ToEntity() ?? throw new Exception($"{nameof(entity.PaymentMethod)} is null");

            return entity;
        }

        public BillingInfoDocument PopulateFromEntity(BillingInfo entity)
        {
            Plan = entity.Plan;
            Amount = entity.Amount;
            Currency = entity.Currency;
            NextBillingDate = entity.NextBillingDate;
            PaymentMethod = PaymentMethodDocument.FromEntity(entity.PaymentMethod)!;

            return this;
        }

        public static BillingInfoDocument? FromEntity(BillingInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new BillingInfoDocument().PopulateFromEntity(entity);
        }
    }
}