using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class StudentContactInfoAddressDocument : IStudentContactInfoAddressDocument
    {
        public string Street { get; set; } = default!;
        public string City { get; set; } = default!;
        public string State { get; set; } = default!;
        public string ZipCode { get; set; } = default!;
        public string Country { get; set; } = default!;
        public bool IsInternational { get; set; }

        public StudentContactInfoAddress ToEntity(StudentContactInfoAddress? entity = default)
        {
            entity ??= new StudentContactInfoAddress();

            entity.Street = Street ?? throw new Exception($"{nameof(entity.Street)} is null");
            entity.City = City ?? throw new Exception($"{nameof(entity.City)} is null");
            entity.State = State ?? throw new Exception($"{nameof(entity.State)} is null");
            entity.ZipCode = ZipCode ?? throw new Exception($"{nameof(entity.ZipCode)} is null");
            entity.Country = Country ?? throw new Exception($"{nameof(entity.Country)} is null");
            entity.IsInternational = IsInternational;

            return entity;
        }

        public StudentContactInfoAddressDocument PopulateFromEntity(StudentContactInfoAddress entity)
        {
            Street = entity.Street;
            City = entity.City;
            State = entity.State;
            ZipCode = entity.ZipCode;
            Country = entity.Country;
            IsInternational = entity.IsInternational;

            return this;
        }

        public static StudentContactInfoAddressDocument? FromEntity(StudentContactInfoAddress? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new StudentContactInfoAddressDocument().PopulateFromEntity(entity);
        }
    }
}