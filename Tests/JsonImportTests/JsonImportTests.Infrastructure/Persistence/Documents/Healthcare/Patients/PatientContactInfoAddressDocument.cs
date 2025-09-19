using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class PatientContactInfoAddressDocument : IPatientContactInfoAddressDocument
    {
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        public string Country { get; set; } = default!;

        public PatientContactInfoAddress ToEntity(PatientContactInfoAddress? entity = default)
        {
            entity ??= new PatientContactInfoAddress();

            entity.Street = Street ?? throw new Exception($"{nameof(entity.Street)} is null");
            entity.City = City ?? throw new Exception($"{nameof(entity.City)} is null");
            entity.State = State ?? throw new Exception($"{nameof(entity.State)} is null");
            entity.ZipCode = ZipCode ?? throw new Exception($"{nameof(entity.ZipCode)} is null");
            entity.Country = Country ?? throw new Exception($"{nameof(entity.Country)} is null");

            return entity;
        }

        public PatientContactInfoAddressDocument PopulateFromEntity(PatientContactInfoAddress entity)
        {
            Street = entity.Street;
            City = entity.City;
            State = entity.State;
            ZipCode = entity.ZipCode;
            Country = entity.Country;

            return this;
        }

        public static PatientContactInfoAddressDocument? FromEntity(PatientContactInfoAddress? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new PatientContactInfoAddressDocument().PopulateFromEntity(entity);
        }
    }
}