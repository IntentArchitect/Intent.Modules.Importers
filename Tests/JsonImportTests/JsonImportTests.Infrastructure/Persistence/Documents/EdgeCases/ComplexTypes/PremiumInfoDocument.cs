using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class PremiumInfoDocument : IPremiumInfoDocument
    {
        public string SubscriptionLevel { get; set; } = default!;
        public List<string> Features { get; set; } = default!;
        IReadOnlyList<string> IPremiumInfoDocument.Features => Features;
        public LimitDocument Limits { get; set; } = default!;
        ILimitDocument IPremiumInfoDocument.Limits => Limits;
        public BillingInfoDocument BillingInfo { get; set; } = default!;
        IBillingInfoDocument IPremiumInfoDocument.BillingInfo => BillingInfo;

        public PremiumInfo ToEntity(PremiumInfo? entity = default)
        {
            entity ??= new PremiumInfo();

            entity.SubscriptionLevel = SubscriptionLevel ?? throw new Exception($"{nameof(entity.SubscriptionLevel)} is null");
            entity.Features = Features ?? throw new Exception($"{nameof(entity.Features)} is null");
            entity.Limits = Limits.ToEntity() ?? throw new Exception($"{nameof(entity.Limits)} is null");
            entity.BillingInfo = BillingInfo.ToEntity() ?? throw new Exception($"{nameof(entity.BillingInfo)} is null");

            return entity;
        }

        public PremiumInfoDocument PopulateFromEntity(PremiumInfo entity)
        {
            SubscriptionLevel = entity.SubscriptionLevel;
            Features = entity.Features.ToList();
            Limits = LimitDocument.FromEntity(entity.Limits)!;
            BillingInfo = BillingInfoDocument.FromEntity(entity.BillingInfo)!;

            return this;
        }

        public static PremiumInfoDocument? FromEntity(PremiumInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PremiumInfoDocument().PopulateFromEntity(entity);
        }
    }
}