using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class RubricDocument : IRubricDocument
    {
        public string Id { get; set; } = default!;
        public Guid CriteriaId { get; set; }
        public string CriteriaName { get; set; } = default!;
        public decimal MaxPoints { get; set; }
        public decimal EarnedPoints { get; set; }
        public string Comments { get; set; } = default!;

        public Rubric ToEntity(Rubric? entity = default)
        {
            entity ??= new Rubric();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.CriteriaId = CriteriaId;
            entity.CriteriaName = CriteriaName ?? throw new Exception($"{nameof(entity.CriteriaName)} is null");
            entity.MaxPoints = MaxPoints;
            entity.EarnedPoints = EarnedPoints;
            entity.Comments = Comments ?? throw new Exception($"{nameof(entity.Comments)} is null");

            return entity;
        }

        public RubricDocument PopulateFromEntity(Rubric entity)
        {
            Id = entity.Id;
            CriteriaId = entity.CriteriaId;
            CriteriaName = entity.CriteriaName;
            MaxPoints = entity.MaxPoints;
            EarnedPoints = entity.EarnedPoints;
            Comments = entity.Comments;

            return this;
        }

        public static RubricDocument? FromEntity(Rubric? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RubricDocument().PopulateFromEntity(entity);
        }
    }
}