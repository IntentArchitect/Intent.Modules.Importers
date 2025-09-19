using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class PaymentMethodDocument : IPaymentMethodDocument
    {
        public string Type { get; set; } = default!;
        public string Last4 { get; set; } = default!;
        public decimal ExpiryMonth { get; set; }
        public decimal ExpiryYear { get; set; }

        public PaymentMethod ToEntity(PaymentMethod? entity = default)
        {
            entity ??= new PaymentMethod();

            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Last4 = Last4 ?? throw new Exception($"{nameof(entity.Last4)} is null");
            entity.ExpiryMonth = ExpiryMonth;
            entity.ExpiryYear = ExpiryYear;

            return entity;
        }

        public PaymentMethodDocument PopulateFromEntity(PaymentMethod entity)
        {
            Type = entity.Type;
            Last4 = entity.Last4;
            ExpiryMonth = entity.ExpiryMonth;
            ExpiryYear = entity.ExpiryYear;

            return this;
        }

        public static PaymentMethodDocument? FromEntity(PaymentMethod? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PaymentMethodDocument().PopulateFromEntity(entity);
        }
    }
}