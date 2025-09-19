using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class OfferingsEnrollmentDocument : IOfferingsEnrollmentDocument
    {
        public decimal MaxStudents { get; set; }
        public decimal EnrolledStudents { get; set; }
        public decimal WaitlistStudents { get; set; }
        public decimal MinEnrollment { get; set; }

        public OfferingsEnrollment ToEntity(OfferingsEnrollment? entity = default)
        {
            entity ??= new OfferingsEnrollment();

            entity.MaxStudents = MaxStudents;
            entity.EnrolledStudents = EnrolledStudents;
            entity.WaitlistStudents = WaitlistStudents;
            entity.MinEnrollment = MinEnrollment;

            return entity;
        }

        public OfferingsEnrollmentDocument PopulateFromEntity(OfferingsEnrollment entity)
        {
            MaxStudents = entity.MaxStudents;
            EnrolledStudents = entity.EnrolledStudents;
            WaitlistStudents = entity.WaitlistStudents;
            MinEnrollment = entity.MinEnrollment;

            return this;
        }

        public static OfferingsEnrollmentDocument? FromEntity(OfferingsEnrollment? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OfferingsEnrollmentDocument().PopulateFromEntity(entity);
        }
    }
}