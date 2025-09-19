using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Students
{
    public interface IAcademicInfoDocument
    {
        DateTime AdmissionDate { get; }
        string StudentType { get; }
        string AcademicStatus { get; }
        string ClassLevel { get; }
        DateTime ExpectedGraduationDate { get; }
        IReadOnlyList<IMajorProgramDocument> MajorPrograms { get; }
        IGPADocument GPA { get; }
        IAcademicAdvisorDocument AcademicAdvisor { get; }
    }
}