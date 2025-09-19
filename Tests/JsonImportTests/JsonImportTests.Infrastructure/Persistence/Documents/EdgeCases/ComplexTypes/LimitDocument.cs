using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.EdgeCases.ComplexTypes;
using JsonImportTests.Domain.Repositories.Documents.EdgeCases.ComplexTypes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.EdgeCases.ComplexTypes
{
    internal class LimitDocument : ILimitDocument
    {
        public decimal APICallsPerDay { get; set; }
        public decimal StorageGB { get; set; }
        public decimal ConcurrentConnections { get; set; }

        public Limit ToEntity(Limit? entity = default)
        {
            entity ??= new Limit();

            entity.APICallsPerDay = APICallsPerDay;
            entity.StorageGB = StorageGB;
            entity.ConcurrentConnections = ConcurrentConnections;

            return entity;
        }

        public LimitDocument PopulateFromEntity(Limit entity)
        {
            APICallsPerDay = entity.APICallsPerDay;
            StorageGB = entity.StorageGB;
            ConcurrentConnections = entity.ConcurrentConnections;

            return this;
        }

        public static LimitDocument? FromEntity(Limit? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new LimitDocument().PopulateFromEntity(entity);
        }
    }
}