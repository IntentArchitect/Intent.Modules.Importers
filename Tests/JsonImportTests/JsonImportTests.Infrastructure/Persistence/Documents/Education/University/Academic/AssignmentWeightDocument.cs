using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class AssignmentWeightDocument : IAssignmentWeightDocument
    {
        public string Id { get; set; } = default!;
        public string Category { get; set; } = default!;
        public decimal Weight { get; set; }

        public AssignmentWeight ToEntity(AssignmentWeight? entity = default)
        {
            entity ??= new AssignmentWeight();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Category = Category ?? throw new Exception($"{nameof(entity.Category)} is null");
            entity.Weight = Weight;

            return entity;
        }

        public AssignmentWeightDocument PopulateFromEntity(AssignmentWeight entity)
        {
            Id = entity.Id;
            Category = entity.Category;
            Weight = entity.Weight;

            return this;
        }

        public static AssignmentWeightDocument? FromEntity(AssignmentWeight? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssignmentWeightDocument().PopulateFromEntity(entity);
        }
    }
}