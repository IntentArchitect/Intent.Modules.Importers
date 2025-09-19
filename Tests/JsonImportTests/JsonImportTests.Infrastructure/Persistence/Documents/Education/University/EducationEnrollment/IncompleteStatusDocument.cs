using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.EducationEnrollment;
using JsonImportTests.Domain.Repositories.Documents.Education.University.EducationEnrollment;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.EducationEnrollment
{
    internal class IncompleteStatusDocument : IIncompleteStatusDocument
    {
        public bool IsIncomplete { get; set; }
        public object IncompleteReason { get; set; } = default!;
        public object CompletionDeadline { get; set; } = default!;
        public List<object> ExtensionRequests { get; set; } = default!;
        IReadOnlyList<object> IIncompleteStatusDocument.ExtensionRequests => ExtensionRequests;

        public IncompleteStatus ToEntity(IncompleteStatus? entity = default)
        {
            entity ??= new IncompleteStatus();

            entity.IsIncomplete = IsIncomplete;
            entity.IncompleteReason = IncompleteReason ?? throw new Exception($"{nameof(entity.IncompleteReason)} is null");
            entity.CompletionDeadline = CompletionDeadline ?? throw new Exception($"{nameof(entity.CompletionDeadline)} is null");
            entity.ExtensionRequests = ExtensionRequests ?? throw new Exception($"{nameof(entity.ExtensionRequests)} is null");

            return entity;
        }

        public IncompleteStatusDocument PopulateFromEntity(IncompleteStatus entity)
        {
            IsIncomplete = entity.IsIncomplete;
            IncompleteReason = entity.IncompleteReason;
            CompletionDeadline = entity.CompletionDeadline;
            ExtensionRequests = entity.ExtensionRequests.ToList();

            return this;
        }

        public static IncompleteStatusDocument? FromEntity(IncompleteStatus? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new IncompleteStatusDocument().PopulateFromEntity(entity);
        }
    }
}