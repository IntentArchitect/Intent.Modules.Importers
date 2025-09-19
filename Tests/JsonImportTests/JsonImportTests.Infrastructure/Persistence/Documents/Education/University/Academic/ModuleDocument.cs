using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class ModuleDocument : IModuleDocument
    {
        public Guid ModuleId { get; set; }
        public string Title { get; set; } = default!;
        public decimal Order { get; set; }
        public List<string> LearningObjectives { get; set; } = default!;
        IReadOnlyList<string> IModuleDocument.LearningObjectives => LearningObjectives;
        public List<TopicDocument> Topics { get; set; } = default!;
        IReadOnlyList<ITopicDocument> IModuleDocument.Topics => Topics;
        public List<AssignmentDocument> Assignments { get; set; } = default!;
        IReadOnlyList<IAssignmentDocument> IModuleDocument.Assignments => Assignments;

        public Module ToEntity(Module? entity = default)
        {
            entity ??= new Module();

            entity.ModuleId = ModuleId;
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.Order = Order;
            entity.LearningObjectives = LearningObjectives ?? throw new Exception($"{nameof(entity.LearningObjectives)} is null");
            entity.Topics = Topics.Select(x => x.ToEntity()).ToList();
            entity.Assignments = Assignments.Select(x => x.ToEntity()).ToList();

            return entity;
        }

        public ModuleDocument PopulateFromEntity(Module entity)
        {
            ModuleId = entity.ModuleId;
            Title = entity.Title;
            Order = entity.Order;
            LearningObjectives = entity.LearningObjectives.ToList();
            Topics = entity.Topics.Select(x => TopicDocument.FromEntity(x)!).ToList();
            Assignments = entity.Assignments.Select(x => AssignmentDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static ModuleDocument? FromEntity(Module? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ModuleDocument().PopulateFromEntity(entity);
        }
    }
}