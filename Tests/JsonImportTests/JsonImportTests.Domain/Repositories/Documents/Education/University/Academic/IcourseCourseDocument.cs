using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IcourseCourseDocument
    {
        string Id { get; }
        string CourseCode { get; }
        string CourseName { get; }
        string CourseTitle { get; }
        string Description { get; }
        decimal Credits { get; }
        string CourseLevel { get; }
        DateTime CreatedDate { get; }
        DateTime LastModified { get; }
        string CreatedBy { get; }
        bool IsActive { get; }
        bool IsOfferedOnline { get; }
        bool IsOfferedInPerson { get; }
        IReadOnlyList<ICoursePrerequisiteDocument> Prerequisites { get; }
        IReadOnlyList<IOfferingDocument> Offerings { get; }
        IReadOnlyList<ILearningOutcomeDocument> LearningOutcomes { get; }
        IDepartmentDocument Department { get; }
        ICourseContentDocument CourseContent { get; }
        IReadOnlyList<ICorequisiteDocument> Corequisites { get; }
        IAccreditationDocument Accreditation { get; }
    }
}