using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class OfferingDocument : IOfferingDocument
    {
        public Guid OfferingId { get; set; }
        public string Semester { get; set; } = default!;
        public decimal Year { get; set; }
        public string Section { get; set; } = default!;
        public ScheduleDocument Schedule { get; set; } = default!;
        IScheduleDocument IOfferingDocument.Schedule => Schedule;
        public OfferingsInstructorDocument Instructor { get; set; } = default!;
        IOfferingsInstructorDocument IOfferingDocument.Instructor => Instructor;
        public GradingPolicyDocument GradingPolicy { get; set; } = default!;
        IGradingPolicyDocument IOfferingDocument.GradingPolicy => GradingPolicy;
        public OfferingsEnrollmentDocument Enrollment { get; set; } = default!;
        IOfferingsEnrollmentDocument IOfferingDocument.Enrollment => Enrollment;

        public Offering ToEntity(Offering? entity = default)
        {
            entity ??= new Offering();

            entity.OfferingId = OfferingId;
            entity.Semester = Semester ?? throw new Exception($"{nameof(entity.Semester)} is null");
            entity.Year = Year;
            entity.Section = Section ?? throw new Exception($"{nameof(entity.Section)} is null");
            entity.Schedule = Schedule.ToEntity() ?? throw new Exception($"{nameof(entity.Schedule)} is null");
            entity.Instructor = Instructor.ToEntity() ?? throw new Exception($"{nameof(entity.Instructor)} is null");
            entity.GradingPolicy = GradingPolicy.ToEntity() ?? throw new Exception($"{nameof(entity.GradingPolicy)} is null");
            entity.Enrollment = Enrollment.ToEntity() ?? throw new Exception($"{nameof(entity.Enrollment)} is null");

            return entity;
        }

        public OfferingDocument PopulateFromEntity(Offering entity)
        {
            OfferingId = entity.OfferingId;
            Semester = entity.Semester;
            Year = entity.Year;
            Section = entity.Section;
            Schedule = ScheduleDocument.FromEntity(entity.Schedule)!;
            Instructor = OfferingsInstructorDocument.FromEntity(entity.Instructor)!;
            GradingPolicy = GradingPolicyDocument.FromEntity(entity.GradingPolicy)!;
            Enrollment = OfferingsEnrollmentDocument.FromEntity(entity.Enrollment)!;

            return this;
        }

        public static OfferingDocument? FromEntity(Offering? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OfferingDocument().PopulateFromEntity(entity);
        }
    }
}