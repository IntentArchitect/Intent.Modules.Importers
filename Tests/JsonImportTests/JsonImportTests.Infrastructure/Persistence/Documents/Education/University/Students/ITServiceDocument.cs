using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class ITServiceDocument : IITServiceDocument
    {
        public string Id { get; set; } = default!;
        public string ServiceType { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ITService ToEntity(ITService? entity = default)
        {
            entity ??= new ITService();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.ServiceType = ServiceType ?? throw new Exception($"{nameof(entity.ServiceType)} is null");
            entity.IsActive = IsActive;
            entity.StartDate = StartDate;
            entity.EndDate = EndDate;

            return entity;
        }

        public ITServiceDocument PopulateFromEntity(ITService entity)
        {
            Id = entity.Id;
            ServiceType = entity.ServiceType;
            IsActive = entity.IsActive;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;

            return this;
        }

        public static ITServiceDocument? FromEntity(ITService? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ITServiceDocument().PopulateFromEntity(entity);
        }
    }
}