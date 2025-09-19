using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class HonorDocument : IHonorDocument
    {
        public Guid HonorId { get; set; }
        public string Title { get; set; } = default!;
        public string Organization { get; set; } = default!;
        public DateTime DateReceived { get; set; }
        public string Description { get; set; } = default!;

        public Honor ToEntity(Honor? entity = default)
        {
            entity ??= new Honor();

            entity.HonorId = HonorId;
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.Organization = Organization ?? throw new Exception($"{nameof(entity.Organization)} is null");
            entity.DateReceived = DateReceived;
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");

            return entity;
        }

        public HonorDocument PopulateFromEntity(Honor entity)
        {
            HonorId = entity.HonorId;
            Title = entity.Title;
            Organization = entity.Organization;
            DateReceived = entity.DateReceived;
            Description = entity.Description;

            return this;
        }

        public static HonorDocument? FromEntity(Honor? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new HonorDocument().PopulateFromEntity(entity);
        }
    }
}