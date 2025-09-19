using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class ComplianceDocument : IComplianceDocument
    {
        public List<object> TradeRestrictions { get; set; } = default!;
        IReadOnlyList<object> IComplianceDocument.TradeRestrictions => TradeRestrictions;
        public bool RequiresApproval { get; set; }
        public object ApprovedBy { get; set; } = default!;
        public object ApprovalTime { get; set; } = default!;
        public List<ComplianceCheckDocument> ComplianceChecks { get; set; } = default!;
        IReadOnlyList<IComplianceCheckDocument> IComplianceDocument.ComplianceChecks => ComplianceChecks;

        public Compliance ToEntity(Compliance? entity = default)
        {
            entity ??= new Compliance();

            entity.TradeRestrictions = TradeRestrictions ?? throw new Exception($"{nameof(entity.TradeRestrictions)} is null");
            entity.RequiresApproval = RequiresApproval;
            entity.ApprovedBy = ApprovedBy ?? throw new Exception($"{nameof(entity.ApprovedBy)} is null");
            entity.ApprovalTime = ApprovalTime ?? throw new Exception($"{nameof(entity.ApprovalTime)} is null");
            entity.ComplianceChecks = ComplianceChecks.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ComplianceDocument PopulateFromEntity(Compliance entity)
        {
            TradeRestrictions = entity.TradeRestrictions.ToList();
            RequiresApproval = entity.RequiresApproval;
            ApprovedBy = entity.ApprovedBy;
            ApprovalTime = entity.ApprovalTime;
            ComplianceChecks = entity.ComplianceChecks.Select(x => ComplianceCheckDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ComplianceDocument? FromEntity(Compliance? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ComplianceDocument().PopulateFromEntity(entity);
        }
    }
}