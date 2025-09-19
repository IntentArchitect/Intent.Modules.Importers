using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class AuditTrailDocument : IAuditTrailDocument
    {
        public string CreatedBy { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public string LastModifiedBy { get; set; } = default!;
        public DateTime LastModifiedAt { get; set; }
        public decimal Version { get; set; }
        public string Source { get; set; } = default!;

        public AuditTrail ToEntity(AuditTrail? entity = default)
        {
            entity ??= new AuditTrail();

            entity.CreatedBy = CreatedBy ?? throw new Exception($"{nameof(entity.CreatedBy)} is null");
            entity.CreatedAt = CreatedAt;
            entity.LastModifiedBy = LastModifiedBy ?? throw new Exception($"{nameof(entity.LastModifiedBy)} is null");
            entity.LastModifiedAt = LastModifiedAt;
            entity.Version = Version;
            entity.Source = Source ?? throw new Exception($"{nameof(entity.Source)} is null");

            return entity;
        }

        public AuditTrailDocument PopulateFromEntity(AuditTrail entity)
        {
            CreatedBy = entity.CreatedBy;
            CreatedAt = entity.CreatedAt;
            LastModifiedBy = entity.LastModifiedBy;
            LastModifiedAt = entity.LastModifiedAt;
            Version = entity.Version;
            Source = entity.Source;

            return this;
        }

        public static AuditTrailDocument? FromEntity(AuditTrail? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AuditTrailDocument().PopulateFromEntity(entity);
        }
    }
}