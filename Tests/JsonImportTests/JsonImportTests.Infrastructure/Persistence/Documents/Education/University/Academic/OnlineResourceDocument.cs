using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class OnlineResourceDocument : IOnlineResourceDocument
    {
        public string Id { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string URL { get; set; } = default!;
        public string Type { get; set; } = default!;
        public bool AccessRequired { get; set; }

        public OnlineResource ToEntity(OnlineResource? entity = default)
        {
            entity ??= new OnlineResource();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.URL = URL ?? throw new Exception($"{nameof(entity.URL)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.AccessRequired = AccessRequired;

            return entity;
        }

        public OnlineResourceDocument PopulateFromEntity(OnlineResource entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            URL = entity.URL;
            Type = entity.Type;
            AccessRequired = entity.AccessRequired;

            return this;
        }

        public static OnlineResourceDocument? FromEntity(OnlineResource? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OnlineResourceDocument().PopulateFromEntity(entity);
        }
    }
}