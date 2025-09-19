using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class WithdrawalInfoDocument : IWithdrawalInfoDocument
    {
        public object WithdrawalDate { get; set; } = default!;
        public object WithdrawalReason { get; set; } = default!;
        public object WithdrawalType { get; set; } = default!;
        public object RefundPercentage { get; set; } = default!;
        public object WithdrawnBy { get; set; } = default!;
        public object AcademicImpact { get; set; } = default!;

        public WithdrawalInfo ToEntity(WithdrawalInfo? entity = default)
        {
            entity ??= new WithdrawalInfo();

            entity.WithdrawalDate = WithdrawalDate ?? throw new Exception($"{nameof(entity.WithdrawalDate)} is null");
            entity.WithdrawalReason = WithdrawalReason ?? throw new Exception($"{nameof(entity.WithdrawalReason)} is null");
            entity.WithdrawalType = WithdrawalType ?? throw new Exception($"{nameof(entity.WithdrawalType)} is null");
            entity.RefundPercentage = RefundPercentage ?? throw new Exception($"{nameof(entity.RefundPercentage)} is null");
            entity.WithdrawnBy = WithdrawnBy ?? throw new Exception($"{nameof(entity.WithdrawnBy)} is null");
            entity.AcademicImpact = AcademicImpact ?? throw new Exception($"{nameof(entity.AcademicImpact)} is null");

            return entity;
        }

        public WithdrawalInfoDocument PopulateFromEntity(WithdrawalInfo entity)
        {
            WithdrawalDate = entity.WithdrawalDate;
            WithdrawalReason = entity.WithdrawalReason;
            WithdrawalType = entity.WithdrawalType;
            RefundPercentage = entity.RefundPercentage;
            WithdrawnBy = entity.WithdrawnBy;
            AcademicImpact = entity.AcademicImpact;

            return this;
        }

        public static WithdrawalInfoDocument? FromEntity(WithdrawalInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new WithdrawalInfoDocument().PopulateFromEntity(entity);
        }
    }
}