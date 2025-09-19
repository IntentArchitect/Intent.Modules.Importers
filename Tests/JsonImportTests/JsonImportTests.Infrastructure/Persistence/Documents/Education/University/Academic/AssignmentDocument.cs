using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class AssignmentDocument : IAssignmentDocument
    {
        public Guid AssignmentId { get; set; }
        public string Title { get; set; } = default!;
        public string Type { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public decimal Points { get; set; }
        public string Instructions { get; set; } = default!;

        public Assignment ToEntity(Assignment? entity = default)
        {
            entity ??= new Assignment();

            entity.AssignmentId = AssignmentId;
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.DueDate = DueDate;
            entity.Points = Points;
            entity.Instructions = Instructions ?? throw new Exception($"{nameof(entity.Instructions)} is null");

            return entity;
        }

        public AssignmentDocument PopulateFromEntity(Assignment entity)
        {
            AssignmentId = entity.AssignmentId;
            Title = entity.Title;
            Type = entity.Type;
            DueDate = entity.DueDate;
            Points = entity.Points;
            Instructions = entity.Instructions;

            return this;
        }

        public static AssignmentDocument? FromEntity(Assignment? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AssignmentDocument().PopulateFromEntity(entity);
        }
    }
}