using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class OrganizationDocument : IOrganizationDocument
    {
        public Guid OrganizationId { get; set; }
        public string Name { get; set; } = default!;
        public string Role { get; set; } = default!;
        public DateTime JoinDate { get; set; }
        public bool IsActive { get; set; }

        public Organization ToEntity(Organization? entity = default)
        {
            entity ??= new Organization();

            entity.OrganizationId = OrganizationId;
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Role = Role ?? throw new Exception($"{nameof(entity.Role)} is null");
            entity.JoinDate = JoinDate;
            entity.IsActive = IsActive;

            return entity;
        }

        public OrganizationDocument PopulateFromEntity(Organization entity)
        {
            OrganizationId = entity.OrganizationId;
            Name = entity.Name;
            Role = entity.Role;
            JoinDate = entity.JoinDate;
            IsActive = entity.IsActive;

            return this;
        }

        public static OrganizationDocument? FromEntity(Organization? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new OrganizationDocument().PopulateFromEntity(entity);
        }
    }
}