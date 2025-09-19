using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class ComplianceCheckDocument : IComplianceCheckDocument
    {
        public string Id { get; set; } = default!;
        public string CheckType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public DateTime CheckedAt { get; set; }

        public ComplianceCheck ToEntity(ComplianceCheck? entity = default)
        {
            entity ??= new ComplianceCheck();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CheckType = CheckType ?? throw new Exception($"{nameof(entity.CheckType)} is null");
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");
            entity.CheckedAt = CheckedAt;

            return entity;
        }

        public ComplianceCheckDocument PopulateFromEntity(ComplianceCheck entity)
        {
            Id = entity.Id;
            CheckType = entity.CheckType;
            Status = entity.Status;
            CheckedAt = entity.CheckedAt;

            return this;
        }

        public static ComplianceCheckDocument? FromEntity(ComplianceCheck? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ComplianceCheckDocument().PopulateFromEntity(entity);
        }
    }
}