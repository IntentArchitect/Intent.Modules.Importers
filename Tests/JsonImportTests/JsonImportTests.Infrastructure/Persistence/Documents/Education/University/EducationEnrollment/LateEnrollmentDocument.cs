using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class LateEnrollmentDocument : ILateEnrollmentDocument
    {
        public bool IsLateEnrollment { get; set; }
        public decimal LateEnrollmentFee { get; set; }
        public bool ApprovalRequired { get; set; }
        public object ApprovedBy { get; set; } = default!;

        public LateEnrollment ToEntity(LateEnrollment? entity = default)
        {
            entity ??= new LateEnrollment();

            entity.IsLateEnrollment = IsLateEnrollment;
            entity.LateEnrollmentFee = LateEnrollmentFee;
            entity.ApprovalRequired = ApprovalRequired;
            entity.ApprovedBy = ApprovedBy ?? throw new Exception($"{nameof(entity.ApprovedBy)} is null");

            return entity;
        }

        public LateEnrollmentDocument PopulateFromEntity(LateEnrollment entity)
        {
            IsLateEnrollment = entity.IsLateEnrollment;
            LateEnrollmentFee = entity.LateEnrollmentFee;
            ApprovalRequired = entity.ApprovalRequired;
            ApprovedBy = entity.ApprovedBy;

            return this;
        }

        public static LateEnrollmentDocument? FromEntity(LateEnrollment? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new LateEnrollmentDocument().PopulateFromEntity(entity);
        }
    }
}