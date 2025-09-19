using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class SpecialCircumstanceDocument : ISpecialCircumstanceDocument
    {
        public LateEnrollmentDocument LateEnrollment { get; set; } = default!;
        ILateEnrollmentDocument ISpecialCircumstanceDocument.LateEnrollment => LateEnrollment;
        public IncompleteStatusDocument IncompleteStatus { get; set; } = default!;
        IIncompleteStatusDocument ISpecialCircumstanceDocument.IncompleteStatus => IncompleteStatus;
        public AuditStatusDocument AuditStatus { get; set; } = default!;
        IAuditStatusDocument ISpecialCircumstanceDocument.AuditStatus => AuditStatus;

        public SpecialCircumstance ToEntity(SpecialCircumstance? entity = default)
        {
            entity ??= new SpecialCircumstance();
            entity.LateEnrollment = LateEnrollment.ToEntity() ?? throw new Exception($"{nameof(entity.LateEnrollment)} is null");
            entity.IncompleteStatus = IncompleteStatus.ToEntity() ?? throw new Exception($"{nameof(entity.IncompleteStatus)} is null");
            entity.AuditStatus = AuditStatus.ToEntity() ?? throw new Exception($"{nameof(entity.AuditStatus)} is null");

            return entity;
        }

        public SpecialCircumstanceDocument PopulateFromEntity(SpecialCircumstance entity)
        {
            LateEnrollment = LateEnrollmentDocument.FromEntity(entity.LateEnrollment)!;
            IncompleteStatus = IncompleteStatusDocument.FromEntity(entity.IncompleteStatus)!;
            AuditStatus = AuditStatusDocument.FromEntity(entity.AuditStatus)!;

            return this;
        }

        public static SpecialCircumstanceDocument? FromEntity(SpecialCircumstance? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SpecialCircumstanceDocument().PopulateFromEntity(entity);
        }
    }
}