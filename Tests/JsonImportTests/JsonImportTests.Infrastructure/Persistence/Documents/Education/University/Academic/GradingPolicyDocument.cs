using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class GradingPolicyDocument : IGradingPolicyDocument
    {
        public List<GradingScaleDocument> GradingScale { get; set; } = default!;
        IReadOnlyList<IGradingScaleDocument> IGradingPolicyDocument.GradingScale => GradingScale;
        public List<AssignmentWeightDocument> AssignmentWeights { get; set; } = default!;
        IReadOnlyList<IAssignmentWeightDocument> IGradingPolicyDocument.AssignmentWeights => AssignmentWeights;

        public GradingPolicy ToEntity(GradingPolicy? entity = default)
        {
            entity ??= new GradingPolicy();
            entity.GradingScale = GradingScale.Select(x => x.ToEntity()).ToList();
            entity.AssignmentWeights = AssignmentWeights.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public GradingPolicyDocument PopulateFromEntity(GradingPolicy entity)
        {
            GradingScale = entity.GradingScale.Select(x => GradingScaleDocument.FromEntity(x)!).ToList();
            AssignmentWeights = entity.AssignmentWeights.Select(x => AssignmentWeightDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static GradingPolicyDocument? FromEntity(GradingPolicy? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GradingPolicyDocument().PopulateFromEntity(entity);
        }
    }
}