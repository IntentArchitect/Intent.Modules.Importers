using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Patients;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Patients;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Patients
{
    internal class ChronicConditionDocument : IChronicConditionDocument
    {
        public string Id { get; set; } = default!;
        public string Condition { get; set; } = default!;
        public DateTime DiagnosedDate { get; set; }
        public string Status { get; set; } = default!;

        public ChronicCondition ToEntity(ChronicCondition? entity = default)
        {
            entity ??= new ChronicCondition();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Condition = Condition ?? throw new Exception($"{nameof(entity.Condition)} is null");
            entity.DiagnosedDate = DiagnosedDate;
            entity.Status = Status ?? throw new Exception($"{nameof(entity.Status)} is null");

            return entity;
        }

        public ChronicConditionDocument PopulateFromEntity(ChronicCondition entity)
        {
            Id = entity.Id;
            Condition = entity.Condition;
            DiagnosedDate = entity.DiagnosedDate;
            Status = entity.Status;

            return this;
        }

        public static ChronicConditionDocument? FromEntity(ChronicCondition? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ChronicConditionDocument().PopulateFromEntity(entity);
        }
    }
}