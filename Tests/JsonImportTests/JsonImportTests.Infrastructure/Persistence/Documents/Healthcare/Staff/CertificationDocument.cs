using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Staff;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Staff;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Staff
{
    internal class CertificationDocument : ICertificationDocument
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string IssuingOrganization { get; set; } = default!;
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string CertificationNumber { get; set; } = default!;

        public Certification ToEntity(Certification? entity = default)
        {
            entity ??= new Certification();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.IssuingOrganization = IssuingOrganization ?? throw new Exception($"{nameof(entity.IssuingOrganization)} is null");
            entity.IssueDate = IssueDate;
            entity.ExpirationDate = ExpirationDate;
            entity.CertificationNumber = CertificationNumber ?? throw new Exception($"{nameof(entity.CertificationNumber)} is null");

            return entity;
        }

        public CertificationDocument PopulateFromEntity(Certification entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            IssuingOrganization = entity.IssuingOrganization;
            IssueDate = entity.IssueDate;
            ExpirationDate = entity.ExpirationDate;
            CertificationNumber = entity.CertificationNumber;

            return this;
        }

        public static CertificationDocument? FromEntity(Certification? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new CertificationDocument().PopulateFromEntity(entity);
        }
    }
}