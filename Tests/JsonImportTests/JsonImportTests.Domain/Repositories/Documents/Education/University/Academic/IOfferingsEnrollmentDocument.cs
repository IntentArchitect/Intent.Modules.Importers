using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IOfferingsEnrollmentDocument
    {
        decimal MaxStudents { get; }
        decimal EnrolledStudents { get; }
        decimal WaitlistStudents { get; }
        decimal MinEnrollment { get; }
    }
}