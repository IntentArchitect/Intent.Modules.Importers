using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Healthcare.Clinical;
using JsonImportTests.Domain.Repositories.Documents.Healthcare.Clinical;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Healthcare.Clinical
{
    internal class DiagnosticTestDocument : IDiagnosticTestDocument
    {
        public string Id { get; set; } = default!;
        public Guid TestId { get; set; }
        public string TestName { get; set; } = default!;
        public string TestType { get; set; } = default!;
        public DateTime OrderedDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public string Results { get; set; } = default!;
        public string Interpretation { get; set; } = default!;
        public string OrderingPhysician { get; set; } = default!;

        public DiagnosticTest ToEntity(DiagnosticTest? entity = default)
        {
            entity ??= new DiagnosticTest();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.TestId = TestId;
            entity.TestName = TestName ?? throw new Exception($"{nameof(entity.TestName)} is null");
            entity.TestType = TestType ?? throw new Exception($"{nameof(entity.TestType)} is null");
            entity.OrderedDate = OrderedDate;
            entity.CompletedDate = CompletedDate;
            entity.Results = Results ?? throw new Exception($"{nameof(entity.Results)} is null");
            entity.Interpretation = Interpretation ?? throw new Exception($"{nameof(entity.Interpretation)} is null");
            entity.OrderingPhysician = OrderingPhysician ?? throw new Exception($"{nameof(entity.OrderingPhysician)} is null");

            return entity;
        }

        public DiagnosticTestDocument PopulateFromEntity(DiagnosticTest entity)
        {
            Id = entity.Id;
            TestId = entity.TestId;
            TestName = entity.TestName;
            TestType = entity.TestType;
            OrderedDate = entity.OrderedDate;
            CompletedDate = entity.CompletedDate;
            Results = entity.Results;
            Interpretation = entity.Interpretation;
            OrderingPhysician = entity.OrderingPhysician;

            return this;
        }

        public static DiagnosticTestDocument? FromEntity(DiagnosticTest? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new DiagnosticTestDocument().PopulateFromEntity(entity);
        }
    }
}