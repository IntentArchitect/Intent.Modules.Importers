using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Entities.Public;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Repositories.Public
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IFktableRepository : IEFRepository<Fktable, Fktable>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int fktableid, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Fktable?> FindByIdAsync(int fktableid, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Fktable?> FindByIdAsync(int fktableid, Func<IQueryable<Fktable>, IQueryable<Fktable>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Fktable>> FindByIdsAsync(int[] fktableids, CancellationToken cancellationToken = default);
    }
}