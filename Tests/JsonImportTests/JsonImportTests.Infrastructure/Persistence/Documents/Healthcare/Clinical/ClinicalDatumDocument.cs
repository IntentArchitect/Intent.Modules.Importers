using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class ClinicalDatumDocument : IClinicalDatumDocument
    {
        public string ChiefComplaint { get; set; } = default!;
        public string HistoryOfPresentIllness { get; set; } = default!;
        public ReviewOfSystemDocument ReviewOfSystems { get; set; } = default!;
        IReviewOfSystemDocument IClinicalDatumDocument.ReviewOfSystems => ReviewOfSystems;
        public PhysicalExaminationDocument PhysicalExamination { get; set; } = default!;
        IPhysicalExaminationDocument IClinicalDatumDocument.PhysicalExamination => PhysicalExamination;

        public ClinicalDatum ToEntity(ClinicalDatum? entity = default)
        {
            entity ??= new ClinicalDatum();

            entity.ChiefComplaint = ChiefComplaint ?? throw new Exception($"{nameof(entity.ChiefComplaint)} is null");
            entity.HistoryOfPresentIllness = HistoryOfPresentIllness ?? throw new Exception($"{nameof(entity.HistoryOfPresentIllness)} is null");
            entity.ReviewOfSystems = ReviewOfSystems.ToEntity() ?? throw new Exception($"{nameof(entity.ReviewOfSystems)} is null");
            entity.PhysicalExamination = PhysicalExamination.ToEntity() ?? throw new Exception($"{nameof(entity.PhysicalExamination)} is null");

            return entity;
        }

        public ClinicalDatumDocument PopulateFromEntity(ClinicalDatum entity)
        {
            ChiefComplaint = entity.ChiefComplaint;
            HistoryOfPresentIllness = entity.HistoryOfPresentIllness;
            ReviewOfSystems = ReviewOfSystemDocument.FromEntity(entity.ReviewOfSystems)!;
            PhysicalExamination = PhysicalExaminationDocument.FromEntity(entity.PhysicalExamination)!;

            return this;
        }

        public static ClinicalDatumDocument? FromEntity(ClinicalDatum? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ClinicalDatumDocument().PopulateFromEntity(entity);
        }
    }
}