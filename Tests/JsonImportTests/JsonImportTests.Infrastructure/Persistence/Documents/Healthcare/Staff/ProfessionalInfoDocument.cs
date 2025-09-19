using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class ProfessionalInfoDocument : IProfessionalInfoDocument
    {
        public string Title { get; set; } = default!;
        public string Department { get; set; } = default!;
        public string Specialization { get; set; } = default!;
        public string LicenseNumber { get; set; } = default!;
        public DateTime LicenseExpirationDate { get; set; }
        public decimal YearsOfExperience { get; set; }

        public ProfessionalInfo ToEntity(ProfessionalInfo? entity = default)
        {
            entity ??= new ProfessionalInfo();

            entity.Title = Title ?? throw new Exception($"{nameof(entity.Title)} is null");
            entity.Department = Department ?? throw new Exception($"{nameof(entity.Department)} is null");
            entity.Specialization = Specialization ?? throw new Exception($"{nameof(entity.Specialization)} is null");
            entity.LicenseNumber = LicenseNumber ?? throw new Exception($"{nameof(entity.LicenseNumber)} is null");
            entity.LicenseExpirationDate = LicenseExpirationDate;
            entity.YearsOfExperience = YearsOfExperience;

            return entity;
        }

        public ProfessionalInfoDocument PopulateFromEntity(ProfessionalInfo entity)
        {
            Title = entity.Title;
            Department = entity.Department;
            Specialization = entity.Specialization;
            LicenseNumber = entity.LicenseNumber;
            LicenseExpirationDate = entity.LicenseExpirationDate;
            YearsOfExperience = entity.YearsOfExperience;

            return this;
        }

        public static ProfessionalInfoDocument? FromEntity(ProfessionalInfo? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ProfessionalInfoDocument().PopulateFromEntity(entity);
        }
    }
}