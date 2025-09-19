using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IstudentStudentDocument
    {
        string Id { get; }
        string StudentId { get; }
        DateTime CreatedDate { get; }
        DateTime LastUpdated { get; }
        bool IsActive { get; }
        bool PrivacyConsent { get; }
        ITechnologyDocument Technology { get; }
        IStudentPersonalInfoDocument PersonalInfo { get; }
        IHealthAndServiceDocument HealthAndServices { get; }
        IStudentFinancialInfoDocument FinancialInfo { get; }
        IReadOnlyList<IEnrollmentHistoryDocument> EnrollmentHistory { get; }
        IStudentContactInfoDocument ContactInfo { get; }
        IActivityDocument Activities { get; }
        IAcademicInfoDocument AcademicInfo { get; }
    }
}