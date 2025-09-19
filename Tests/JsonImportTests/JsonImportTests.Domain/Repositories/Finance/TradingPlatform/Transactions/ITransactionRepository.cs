using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities.Finance.TradingPlatform.Transactions;
using JsonImportTests.Domain.Repositories.Documents.Finance.TradingPlatform.Transactions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories.Finance.TradingPlatform.Transactions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ITransactionRepository : ICosmosDBRepository<Transaction, ITransactionDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<Transaction?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Transaction>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}