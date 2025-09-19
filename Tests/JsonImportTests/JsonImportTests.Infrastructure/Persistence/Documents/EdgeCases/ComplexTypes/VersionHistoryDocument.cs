using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class VersionHistoryDocument : IVersionHistoryDocument
    {
        public string Id { get; set; } = default!;
        public string Version { get; set; } = default!;
        public List<string> Changes { get; set; } = default!;
        IReadOnlyList<string> IVersionHistoryDocument.Changes => Changes;
        public DateTime Date { get; set; }
        public string Author { get; set; } = default!;
        public bool Breaking { get; set; }

        public VersionHistory ToEntity(VersionHistory? entity = default)
        {
            entity ??= new VersionHistory();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Version = Version ?? throw new Exception($"{nameof(entity.Version)} is null");
            entity.Changes = Changes ?? throw new Exception($"{nameof(entity.Changes)} is null");
            entity.Date = Date;
            entity.Author = Author ?? throw new Exception($"{nameof(entity.Author)} is null");
            entity.Breaking = Breaking;

            return entity;
        }

        public VersionHistoryDocument PopulateFromEntity(VersionHistory entity)
        {
            Id = entity.Id;
            Version = entity.Version;
            Changes = entity.Changes.ToList();
            Date = entity.Date;
            Author = entity.Author;
            Breaking = entity.Breaking;

            return this;
        }

        public static VersionHistoryDocument? FromEntity(VersionHistory? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VersionHistoryDocument().PopulateFromEntity(entity);
        }
    }
}