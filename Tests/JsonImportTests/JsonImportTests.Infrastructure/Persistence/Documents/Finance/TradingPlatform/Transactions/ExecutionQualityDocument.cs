using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class ExecutionQualityDocument : IExecutionQualityDocument
    {
        public decimal PriceImprovement { get; set; }
        public decimal SpreadCaptured { get; set; }
        public decimal TimeToExecution { get; set; }

        public ExecutionQuality ToEntity(ExecutionQuality? entity = default)
        {
            entity ??= new ExecutionQuality();

            entity.PriceImprovement = PriceImprovement;
            entity.SpreadCaptured = SpreadCaptured;
            entity.TimeToExecution = TimeToExecution;

            return entity;
        }

        public ExecutionQualityDocument PopulateFromEntity(ExecutionQuality entity)
        {
            PriceImprovement = entity.PriceImprovement;
            SpreadCaptured = entity.SpreadCaptured;
            TimeToExecution = entity.TimeToExecution;

            return this;
        }

        public static ExecutionQualityDocument? FromEntity(ExecutionQuality? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new ExecutionQualityDocument().PopulateFromEntity(entity);
        }
    }
}