using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class PractitionerContactInfoDocument : IPractitionerContactInfoDocument
    {
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public PractitionerContactInfoAddressDocument Address { get; set; } = default!;
        IPractitionerContactInfoAddressDocument IPractitionerContactInfoDocument.Address => Address;

        public PractitionerContactInfo ToEntity(PractitionerContactInfo? entity = default)
        {
            entity ??= new PractitionerContactInfo();

            entity.Phone = Phone ?? throw new Exception($"{nameof(entity.Phone)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.Address = Address.ToEntity() ?? throw new Exception($"{nameof(entity.Address)} is null");

            return entity;
        }

        public PractitionerContactInfoDocument PopulateFromEntity(PractitionerContactInfo entity)
        {
            Phone = entity.Phone;
            Email = entity.Email;
            Address = PractitionerContactInfoAddressDocument.FromEntity(entity.Address)!;

            return this;
        }

        public static PractitionerContactInfoDocument? FromEntity(PractitionerContactInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PractitionerContactInfoDocument().PopulateFromEntity(entity);
        }
    }
}