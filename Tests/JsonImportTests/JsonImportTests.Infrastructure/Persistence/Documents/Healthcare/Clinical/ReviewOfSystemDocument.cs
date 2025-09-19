using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class ReviewOfSystemDocument : IReviewOfSystemDocument
    {
        public string Constitutional { get; set; } = default!;
        public string Cardiovascular { get; set; } = default!;
        public string Respiratory { get; set; } = default!;
        public string Gastrointestinal { get; set; } = default!;
        public string Neurological { get; set; } = default!;
        public string Musculoskeletal { get; set; } = default!;

        public ReviewOfSystem ToEntity(ReviewOfSystem? entity = default)
        {
            entity ??= new ReviewOfSystem();

            entity.Constitutional = Constitutional ?? throw new Exception($"{nameof(entity.Constitutional)} is null");
            entity.Cardiovascular = Cardiovascular ?? throw new Exception($"{nameof(entity.Cardiovascular)} is null");
            entity.Respiratory = Respiratory ?? throw new Exception($"{nameof(entity.Respiratory)} is null");
            entity.Gastrointestinal = Gastrointestinal ?? throw new Exception($"{nameof(entity.Gastrointestinal)} is null");
            entity.Neurological = Neurological ?? throw new Exception($"{nameof(entity.Neurological)} is null");
            entity.Musculoskeletal = Musculoskeletal ?? throw new Exception($"{nameof(entity.Musculoskeletal)} is null");

            return entity;
        }

        public ReviewOfSystemDocument PopulateFromEntity(ReviewOfSystem entity)
        {
            Constitutional = entity.Constitutional;
            Cardiovascular = entity.Cardiovascular;
            Respiratory = entity.Respiratory;
            Gastrointestinal = entity.Gastrointestinal;
            Neurological = entity.Neurological;
            Musculoskeletal = entity.Musculoskeletal;

            return this;
        }

        public static ReviewOfSystemDocument? FromEntity(ReviewOfSystem? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ReviewOfSystemDocument().PopulateFromEntity(entity);
        }
    }
}