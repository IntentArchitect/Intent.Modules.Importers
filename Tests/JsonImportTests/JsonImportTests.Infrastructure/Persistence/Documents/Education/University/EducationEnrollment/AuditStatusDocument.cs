using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class AuditStatusDocument : IAuditStatusDocument
    {
        public bool IsAudit { get; set; }
        public object AuditApprovalDate { get; set; } = default!;
        public decimal AuditFee { get; set; }

        public AuditStatus ToEntity(AuditStatus? entity = default)
        {
            entity ??= new AuditStatus();

            entity.IsAudit = IsAudit;
            entity.AuditApprovalDate = AuditApprovalDate ?? throw new Exception($"{nameof(entity.AuditApprovalDate)} is null");
            entity.AuditFee = AuditFee;

            return entity;
        }

        public AuditStatusDocument PopulateFromEntity(AuditStatus entity)
        {
            IsAudit = entity.IsAudit;
            AuditApprovalDate = entity.AuditApprovalDate;
            AuditFee = entity.AuditFee;

            return this;
        }

        public static AuditStatusDocument? FromEntity(AuditStatus? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AuditStatusDocument().PopulateFromEntity(entity);
        }
    }
}