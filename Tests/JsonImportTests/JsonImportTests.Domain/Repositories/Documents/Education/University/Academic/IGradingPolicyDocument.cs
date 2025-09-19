using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Documents.Education.University.Academic
{
    public interface IGradingPolicyDocument
    {
        IReadOnlyList<IGradingScaleDocument> GradingScale { get; }
        IReadOnlyList<IAssignmentWeightDocument> AssignmentWeights { get; }
    }
}