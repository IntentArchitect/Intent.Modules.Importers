using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class TextBookDocument : ITextBookDocument
    {
        public string Id { get; set; } = default!;
        public string ISBN { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Author { get; set; } = default!;
        public string Edition { get; set; } = default!;
        public string Publisher { get; set; } = default!;
        public bool IsRequired { get; set; }

        public TextBook ToEntity(TextBook? entity = default)
        {
            entity ??= new TextBook();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ISBN = ISBN ?? throw new Exception($"{nameof(entity.ISBN)} is null");
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.Author = Author ?? throw new Exception($"{nameof(entity.Author)} is null");
            entity.Edition = Edition ?? throw new Exception($"{nameof(entity.Edition)} is null");
            entity.Publisher = Publisher ?? throw new Exception($"{nameof(entity.Publisher)} is null");
            entity.IsRequired = IsRequired;

            return entity;
        }

        public TextBookDocument PopulateFromEntity(TextBook entity)
        {
            Id = entity.Id;
            ISBN = entity.ISBN;
            Title = entity.Title;
            Author = entity.Author;
            Edition = entity.Edition;
            Publisher = entity.Publisher;
            IsRequired = entity.IsRequired;

            return this;
        }

        public static TextBookDocument? FromEntity(TextBook? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new TextBookDocument().PopulateFromEntity(entity);
        }
    }
}