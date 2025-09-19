using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IOfferingDocument
    {
        Guid OfferingId { get; }
        string Semester { get; }
        decimal Year { get; }
        string Section { get; }
        IScheduleDocument Schedule { get; }
        IOfferingsInstructorDocument Instructor { get; }
        IGradingPolicyDocument GradingPolicy { get; }
        IOfferingsEnrollmentDocument Enrollment { get; }
    }
}