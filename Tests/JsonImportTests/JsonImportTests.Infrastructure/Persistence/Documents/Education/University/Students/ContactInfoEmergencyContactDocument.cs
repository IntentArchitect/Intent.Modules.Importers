using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class ContactInfoEmergencyContactDocument : IContactInfoEmergencyContactDocument
    {
        public string Name { get; set; } = default!;
        public string Relationship { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;

        public ContactInfoEmergencyContact ToEntity(ContactInfoEmergencyContact? entity = default)
        {
            entity ??= new ContactInfoEmergencyContact();

            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Relationship = Relationship ?? throw new Exception($"{nameof(entity.Relationship)} is null");
            entity.Phone = Phone ?? throw new Exception($"{nameof(entity.Phone)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");

            return entity;
        }

        public ContactInfoEmergencyContactDocument PopulateFromEntity(ContactInfoEmergencyContact entity)
        {
            Name = entity.Name;
            Relationship = entity.Relationship;
            Phone = entity.Phone;
            Email = entity.Email;

            return this;
        }

        public static ContactInfoEmergencyContactDocument? FromEntity(ContactInfoEmergencyContact? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ContactInfoEmergencyContactDocument().PopulateFromEntity(entity);
        }
    }
}