using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocument", Version = "1.0")]

namespace JsonImportTests.Infrastructure.Persistence.Documents.Finance.TradingPlatform.Transactions
{
    internal class RelatedTransactionDocument : IRelatedTransactionDocument
    {
        public Guid RelatedTransactionId { get; set; }
        public string RelationType { get; set; } = default!;
        public string Description { get; set; } = default!;

        public RelatedTransaction ToEntity(RelatedTransaction? entity = default)
        {
            entity ??= new RelatedTransaction();

            entity.RelatedTransactionId = RelatedTransactionId;
            entity.RelationType = RelationType ?? throw new Exception($"{nameof(entity.RelationType)} is null");
            entity.Description = Description ?? throw new Exception($"{nameof(entity.Description)} is null");

            return entity;
        }

        public RelatedTransactionDocument PopulateFromEntity(RelatedTransaction entity)
        {
            RelatedTransactionId = entity.RelatedTransactionId;
            RelationType = entity.RelationType;
            Description = entity.Description;

            return this;
        }

        public static RelatedTransactionDocument? FromEntity(RelatedTransaction? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new RelatedTransactionDocument().PopulateFromEntity(entity);
        }
    }
}