using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Entities.Schema2;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Repositories.Schema2
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IBanksRepository : IEFRepository<Banks, Banks>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Banks?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Banks?> FindByIdAsync(Guid id, Func<IQueryable<Banks>, IQueryable<Banks>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Banks>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}