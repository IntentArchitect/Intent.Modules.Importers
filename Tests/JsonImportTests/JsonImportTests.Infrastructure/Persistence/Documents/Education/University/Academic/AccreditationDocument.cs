using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Education.University.Academic;
using JsonImportTests.Domain.Repositories.Documents.Education.University.Academic;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Education.University.Academic
{
    internal class AccreditationDocument : IAccreditationDocument
    {
        public string AccreditingBody { get; set; } = default!;
        public string AccreditationLevel { get; set; } = default!;
        public DateTime LastReviewDate { get; set; }
        public DateTime NextReviewDate { get; set; }
        public string Status { get; set; } = default!;

        public Accreditation ToEntity(Accreditation? entity = default)
        {
            entity ??= new Accreditation();

            entity.AccreditingBody = AccreditingBody ?? throw new Exception($"{nameof(entity.AccreditingBody)} is null");
            entity.AccreditationLevel = AccreditationLevel ?? throw new Exception($"{nameof(entity.AccreditationLevel)} is null");
            entity.LastReviewDate = LastReviewDate;
            entity.NextReviewDate = NextReviewDate;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");

            return entity;
        }

        public AccreditationDocument PopulateFromEntity(Accreditation entity)
        {
            AccreditingBody = entity.AccreditingBody;
            AccreditationLevel = entity.AccreditationLevel;
            LastReviewDate = entity.LastReviewDate;
            NextReviewDate = entity.NextReviewDate;
            Status = entity.Status;

            return this;
        }

        public static AccreditationDocument? FromEntity(Accreditation? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new AccreditationDocument().PopulateFromEntity(entity);
        }
    }
}