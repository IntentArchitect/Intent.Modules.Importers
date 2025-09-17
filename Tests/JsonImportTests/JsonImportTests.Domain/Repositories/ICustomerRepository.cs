using Intent.RoslynWeaver.Attributes;
using JsonImportTests.Domain.Entities;
using JsonImportTests.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace JsonImportTests.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerRepository : ICosmosDBRepository<customer, IcustomerDocument>
    {
        [IntentManaged(Mode.Fully)]
        Task<customer?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<customer>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}