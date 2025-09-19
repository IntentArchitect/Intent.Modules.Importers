using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class AmendmentDocument : IAmendmentDocument
    {
        public Guid AmendmentId { get; set; }
        public string AmendmentType { get; set; } = default!;
        public string AmendmentReason { get; set; } = default!;
        public string OriginalValue { get; set; } = default!;
        public string NewValue { get; set; } = default!;
        public string AmendedBy { get; set; } = default!;
        public DateTime AmendedAt { get; set; }

        public Amendment ToEntity(Amendment? entity = default)
        {
            entity ??= new Amendment();

            entity.AmendmentId = AmendmentId;
            entity.AmendmentType = AmendmentType ?? throw new Exception($"{nameof(entity.AmendmentType)} is null");
            entity.AmendmentReason = AmendmentReason ?? throw new Exception($"{nameof(entity.AmendmentReason)} is null");
            entity.OriginalValue = OriginalValue ?? throw new Exception($"{nameof(entity.OriginalValue)} is null");
            entity.NewValue = NewValue ?? throw new Exception($"{nameof(entity.NewValue)} is null");
            entity.AmendedBy = AmendedBy ?? throw new Exception($"{nameof(entity.AmendedBy)} is null");
            entity.AmendedAt = AmendedAt;

            return entity;
        }

        public AmendmentDocument PopulateFromEntity(Amendment entity)
        {
            AmendmentId = entity.AmendmentId;
            AmendmentType = entity.AmendmentType;
            AmendmentReason = entity.AmendmentReason;
            OriginalValue = entity.OriginalValue;
            NewValue = entity.NewValue;
            AmendedBy = entity.AmendedBy;
            AmendedAt = entity.AmendedAt;

            return this;
        }

        public static AmendmentDocument? FromEntity(Amendment? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AmendmentDocument().PopulateFromEntity(entity);
        }
    }
}