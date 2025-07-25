using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Entities.PsgSchema2;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Repositories.PsgSchema2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBankRepository : IEFRepository<Bank, Bank>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Bank?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Bank?> FindByIdAsync(Guid id, Func<IQueryable<Bank>, IQueryable<Bank>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Bank>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}