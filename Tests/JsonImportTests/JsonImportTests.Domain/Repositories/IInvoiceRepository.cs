using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities;
using JsonImportTests.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IInvoiceRepository : ICosmosDBRepository<invoice, IinvoiceDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<invoice?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<invoice>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}