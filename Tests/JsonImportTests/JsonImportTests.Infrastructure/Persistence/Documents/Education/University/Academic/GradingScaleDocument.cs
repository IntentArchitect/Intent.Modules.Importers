using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class GradingScaleDocument : IGradingScaleDocument
    {
        public string Id { get; set; } = default!;
        public string Grade { get; set; } = default!;
        public decimal MinPercentage { get; set; }
        public decimal MaxPercentage { get; set; }
        public decimal GradePoints { get; set; }

        public GradingScale ToEntity(GradingScale? entity = default)
        {
            entity ??= new GradingScale();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Grade = Grade ?? throw new Exception($"{nameof(entity.Grade)} is null");
            entity.MinPercentage = MinPercentage;
            entity.MaxPercentage = MaxPercentage;
            entity.GradePoints = GradePoints;

            return entity;
        }

        public GradingScaleDocument PopulateFromEntity(GradingScale entity)
        {
            Id = entity.Id;
            Grade = entity.Grade;
            MinPercentage = entity.MinPercentage;
            MaxPercentage = entity.MaxPercentage;
            GradePoints = entity.GradePoints;

            return this;
        }

        public static GradingScaleDocument? FromEntity(GradingScale? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new GradingScaleDocument().PopulateFromEntity(entity);
        }
    }
}