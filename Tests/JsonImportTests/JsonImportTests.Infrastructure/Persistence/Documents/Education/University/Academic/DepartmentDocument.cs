using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class DepartmentDocument : IDepartmentDocument
    {
        public Guid DepartmentId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Faculty { get; set; } = default!;

        public Department ToEntity(Department? entity = default)
        {
            entity ??= new Department();

            entity.DepartmentId = DepartmentId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Code = Code ?? throw new Exception($"{nameof(entity.Code)} is null");
            entity.Faculty = Faculty ?? throw new Exception($"{nameof(entity.Faculty)} is null");

            return entity;
        }

        public DepartmentDocument PopulateFromEntity(Department entity)
        {
            DepartmentId = entity.DepartmentId;
            Name = entity.Name;
            Code = entity.Code;
            Faculty = entity.Faculty;

            return this;
        }

        public static DepartmentDocument? FromEntity(Department? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DepartmentDocument().PopulateFromEntity(entity);
        }
    }
}