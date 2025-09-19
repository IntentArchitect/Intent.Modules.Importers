using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class StudentContactInfoDocument : IStudentContactInfoDocument
    {
        public string Email { get; set; } = default!;
        public string AlternateEmail { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public ContactInfoEmergencyContactDocument EmergencyContact { get; set; } = default!;
        IContactInfoEmergencyContactDocument IStudentContactInfoDocument.EmergencyContact => EmergencyContact;
        public StudentContactInfoAddressDocument Address { get; set; } = default!;
        IStudentContactInfoAddressDocument IStudentContactInfoDocument.Address => Address;

        public StudentContactInfo ToEntity(StudentContactInfo? entity = default)
        {
            entity ??= new StudentContactInfo();

            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.AlternateEmail = AlternateEmail ?? throw new Exception($"{nameof(entity.AlternateEmail)} is null");
            entity.Phone = Phone ?? throw new Exception($"{nameof(entity.Phone)} is null");
            entity.EmergencyContact = EmergencyContact.ToEntity() ?? throw new Exception($"{nameof(entity.EmergencyContact)} is null");
            entity.Address = Address.ToEntity() ?? throw new Exception($"{nameof(entity.Address)} is null");

            return entity;
        }

        public StudentContactInfoDocument PopulateFromEntity(StudentContactInfo entity)
        {
            Email = entity.Email;
            AlternateEmail = entity.AlternateEmail;
            Phone = entity.Phone;
            EmergencyContact = ContactInfoEmergencyContactDocument.FromEntity(entity.EmergencyContact)!;
            Address = StudentContactInfoAddressDocument.FromEntity(entity.Address)!;

            return this;
        }

        public static StudentContactInfoDocument? FromEntity(StudentContactInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new StudentContactInfoDocument().PopulateFromEntity(entity);
        }
    }
}