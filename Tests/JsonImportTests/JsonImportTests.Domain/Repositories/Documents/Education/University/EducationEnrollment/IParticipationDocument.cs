using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment
{
    public interface IParticipationDocument
    {
        string ParticipationGrade { get; }
        decimal ParticipationPoints { get; }
        decimal MaxParticipationPoints { get; }
        string ParticipationComments { get; }
        string EngagementLevel { get; }
    }
}