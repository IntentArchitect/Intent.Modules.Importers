using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class VisitDetailsDiagnosisDocument : IVisitDetailsDiagnosisDocument
    {
        public string Id { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Certainty { get; set; } = default!;

        public VisitDetailsDiagnosis ToEntity(VisitDetailsDiagnosis? entity = default)
        {
            entity ??= new VisitDetailsDiagnosis();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Code = Code ?? throw new Exception($"{nameof(entity.Code)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");
            entity.Type = Type ?? throw new Exception($"{nameof(entity.Type)} is null");
            entity.Certainty = Certainty ?? throw new Exception($"{nameof(entity.Certainty)} is null");

            return entity;
        }

        public VisitDetailsDiagnosisDocument PopulateFromEntity(VisitDetailsDiagnosis entity)
        {
            Id = entity.Id;
            Code = entity.Code;
            Description = entity.Description;
            Type = entity.Type;
            Certainty = entity.Certainty;

            return this;
        }

        public static VisitDetailsDiagnosisDocument? FromEntity(VisitDetailsDiagnosis? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new VisitDetailsDiagnosisDocument().PopulateFromEntity(entity);
        }
    }
}