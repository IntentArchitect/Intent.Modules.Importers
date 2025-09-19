using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IEnrollmentPrerequisiteDocument
    {
        bool AllPrerequisitesMet { get; }
        IReadOnlyList<IWaiverDocument> Waivers { get; }
        IReadOnlyList<IPrerequisiteCheckDocument> PrerequisiteChecks { get; }
    }
}