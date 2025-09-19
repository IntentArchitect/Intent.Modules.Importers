using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class CreatedByDocument : ICreatedByDocument
    {
        public Guid PractitionerId { get; set; }
        public string Name { get; set; } = default!;
        public string Title { get; set; } = default!;

        public CreatedBy ToEntity(CreatedBy? entity = default)
        {
            entity ??= new CreatedBy();

            entity.PractitionerId = PractitionerId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");

            return entity;
        }

        public CreatedByDocument PopulateFromEntity(CreatedBy entity)
        {
            PractitionerId = entity.PractitionerId;
            Name = entity.Name;
            Title = entity.Title;

            return this;
        }

        public static CreatedByDocument? FromEntity(CreatedBy? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CreatedByDocument().PopulateFromEntity(entity);
        }
    }
}