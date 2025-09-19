using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class EnrollmentFinancialInfoDocument : IEnrollmentFinancialInfoDocument
    {
        public decimal TuitionCharged { get; set; }
        public decimal FeesCharged { get; set; }
        public decimal FinancialAidApplied { get; set; }
        public decimal OutstandingBalance { get; set; }
        public string PaymentStatus { get; set; } = default!;
        public bool RefundEligible { get; set; }
        public decimal RefundAmount { get; set; }

        public EnrollmentFinancialInfo ToEntity(EnrollmentFinancialInfo? entity = default)
        {
            entity ??= new EnrollmentFinancialInfo();

            entity.TuitionCharged = TuitionCharged;
            entity.FeesCharged = FeesCharged;
            entity.FinancialAidApplied = FinancialAidApplied;
            entity.OutstandingBalance = OutstandingBalance;
            entity.PaymentStatus = PaymentStatus ?? throw new Exception($"{nameof(entity.PaymentStatus)} is null");
            entity.RefundEligible = RefundEligible;
            entity.RefundAmount = RefundAmount;

            return entity;
        }

        public EnrollmentFinancialInfoDocument PopulateFromEntity(EnrollmentFinancialInfo entity)
        {
            TuitionCharged = entity.TuitionCharged;
            FeesCharged = entity.FeesCharged;
            FinancialAidApplied = entity.FinancialAidApplied;
            OutstandingBalance = entity.OutstandingBalance;
            PaymentStatus = entity.PaymentStatus;
            RefundEligible = entity.RefundEligible;
            RefundAmount = entity.RefundAmount;

            return this;
        }

        public static EnrollmentFinancialInfoDocument? FromEntity(EnrollmentFinancialInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new EnrollmentFinancialInfoDocument().PopulateFromEntity(entity);
        }
    }
}