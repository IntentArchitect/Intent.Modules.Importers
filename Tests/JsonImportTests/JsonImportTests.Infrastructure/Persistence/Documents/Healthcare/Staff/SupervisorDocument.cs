using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class SupervisorDocument : ISupervisorDocument
    {
        public Guid SupervisorId { get; set; }
        public string Name { get; set; } = default!;
        public string Title { get; set; } = default!;

        public Supervisor ToEntity(Supervisor? entity = default)
        {
            entity ??= new Supervisor();

            entity.SupervisorId = SupervisorId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");

            return entity;
        }

        public SupervisorDocument PopulateFromEntity(Supervisor entity)
        {
            SupervisorId = entity.SupervisorId;
            Name = entity.Name;
            Title = entity.Title;

            return this;
        }

        public static SupervisorDocument? FromEntity(Supervisor? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new SupervisorDocument().PopulateFromEntity(entity);
        }
    }
}