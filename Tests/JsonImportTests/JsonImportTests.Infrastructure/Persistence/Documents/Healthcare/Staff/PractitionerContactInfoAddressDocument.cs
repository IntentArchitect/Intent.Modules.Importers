using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class PractitionerContactInfoAddressDocument : IPractitionerContactInfoAddressDocument
    {
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;

        public PractitionerContactInfoAddress ToEntity(PractitionerContactInfoAddress? entity = default)
        {
            entity ??= new PractitionerContactInfoAddress();

            entity.Street = Street ?? throw new Exception($"{nameof(entity.Street)} is null");
            entity.City = City ?? throw new Exception($"{nameof(entity.City)} is null");
            entity.State = State ?? throw new Exception($"{nameof(entity.State)} is null");
            entity.ZipCode = ZipCode ?? throw new Exception($"{nameof(entity.ZipCode)} is null");

            return entity;
        }

        public PractitionerContactInfoAddressDocument PopulateFromEntity(PractitionerContactInfoAddress entity)
        {
            Street = entity.Street;
            City = entity.City;
            State = entity.State;
            ZipCode = entity.ZipCode;

            return this;
        }

        public static PractitionerContactInfoAddressDocument? FromEntity(PractitionerContactInfoAddress? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PractitionerContactInfoAddressDocument().PopulateFromEntity(entity);
        }
    }
}