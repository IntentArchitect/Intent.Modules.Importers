using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class AssessmentDocument : IAssessmentDocument
    {
        public Guid AssessmentId { get; set; }
        public string Type { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal MaxPoints { get; set; }
        public decimal EarnedPoints { get; set; }
        public decimal Percentage { get; set; }
        public decimal Weight { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime GradedDate { get; set; }
        public string Comments { get; set; } = default!;
        public List<RubricDocument> Rubric { get; set; } = default!;
        IReadOnlyList<IRubricDocument> IAssessmentDocument.Rubric => Rubric;

        public Assessment ToEntity(Assessment? entity = default)
        {
            entity ??= new Assessment();

            entity.AssessmentId = AssessmentId;
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.MaxPoints = MaxPoints;
            entity.EarnedPoints = EarnedPoints;
            entity.Percentage = Percentage;
            entity.Weight = Weight;
            entity.DueDate = DueDate;
            entity.SubmittedDate = SubmittedDate;
            entity.GradedDate = GradedDate;
            entity.Comments = Comments ?? throw new Exception($"{nameof(entity.Comments)} is null");
            entity.Rubric = Rubric.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public AssessmentDocument PopulateFromEntity(Assessment entity)
        {
            AssessmentId = entity.AssessmentId;
            Type = entity.Type;
            Name = entity.Name;
            MaxPoints = entity.MaxPoints;
            EarnedPoints = entity.EarnedPoints;
            Percentage = entity.Percentage;
            Weight = entity.Weight;
            DueDate = entity.DueDate;
            SubmittedDate = entity.SubmittedDate;
            GradedDate = entity.GradedDate;
            Comments = entity.Comments;
            Rubric = entity.Rubric.Select(x => RubricDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static AssessmentDocument? FromEntity(Assessment? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssessmentDocument().PopulateFromEntity(entity);
        }
    }
}