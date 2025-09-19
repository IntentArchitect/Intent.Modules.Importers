using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class AcademicAdvisorDocument : IAcademicAdvisorDocument
    {
        public Guid AdvisorId { get; set; }
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Department { get; set; } = default!;
        public DateTime AssignedDate { get; set; }

        public AcademicAdvisor ToEntity(AcademicAdvisor? entity = default)
        {
            entity ??= new AcademicAdvisor();

            entity.AdvisorId = AdvisorId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.Department = Department ?? throw new Exception($"{nameof(entity.Department)} is null");
            entity.AssignedDate = AssignedDate;

            return entity;
        }

        public AcademicAdvisorDocument PopulateFromEntity(AcademicAdvisor entity)
        {
            AdvisorId = entity.AdvisorId;
            Name = entity.Name;
            Email = entity.Email;
            Department = entity.Department;
            AssignedDate = entity.AssignedDate;

            return this;
        }

        public static AcademicAdvisorDocument? FromEntity(AcademicAdvisor? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AcademicAdvisorDocument().PopulateFromEntity(entity);
        }
    }
}