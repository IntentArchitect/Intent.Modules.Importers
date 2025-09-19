using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Students;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Students;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Students
{
    internal class DisabilityDocument : IDisabilityDocument
    {
        public string Id { get; set; } = default!;
        public string DisabilityType { get; set; } = default!;
        public List<string> AccommodationsNeeded { get; set; } = default!;
        IReadOnlyList<string> IDisabilityDocument.AccommodationsNeeded => AccommodationsNeeded;
        public DateTime ApprovedDate { get; set; }

        public Disability ToEntity(Disability? entity = default)
        {
            entity ??= new Disability();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.DisabilityType = DisabilityType ?? throw new Exception($"{nameof(entity.DisabilityType)} is null");
            entity.AccommodationsNeeded = AccommodationsNeeded ?? throw new Exception($"{nameof(entity.AccommodationsNeeded)} is null");
            entity.ApprovedDate = ApprovedDate;

            return entity;
        }

        public DisabilityDocument PopulateFromEntity(Disability entity)
        {
            Id = entity.Id;
            DisabilityType = entity.DisabilityType;
            AccommodationsNeeded = entity.AccommodationsNeeded.ToList();
            ApprovedDate = entity.ApprovedDate;

            return this;
        }

        public static DisabilityDocument? FromEntity(Disability? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DisabilityDocument().PopulateFromEntity(entity);
        }
    }
}