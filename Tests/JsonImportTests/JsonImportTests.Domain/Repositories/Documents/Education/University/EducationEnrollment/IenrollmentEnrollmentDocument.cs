using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IenrollmentEnrollmentDocument
    {
        string Id { get; }
        Guid StudentId { get; }
        Guid CourseOfferingId { get; }
        Guid SemesterId { get; }
        DateTime EnrollmentDate { get; }
        string EnrollmentStatus { get; }
        string EnrollmentType { get; }
        DateTime CreatedDate { get; }
        DateTime LastModified { get; }
        string ModifiedBy { get; }
        decimal Version { get; }
        bool IsActive { get; }
        IWithdrawalInfoDocument WithdrawalInfo { get; }
        IEnrollmentStudentDocument Student { get; }
        ISpecialCircumstanceDocument SpecialCircumstances { get; }
        ISemesterDocument Semester { get; }
        IEnrollmentPrerequisiteDocument Prerequisites { get; }
        IParticipationDocument Participation { get; }
        IEnrollmentInstructorDocument Instructor { get; }
        IGradingInfoDocument GradingInfo { get; }
        IEnrollmentFinancialInfoDocument FinancialInfo { get; }
        IEnrollmentCourseDocument Course { get; }
        IAttendanceDocument Attendance { get; }
        IReadOnlyList<IAssessmentDocument> Assessments { get; }
    }
}