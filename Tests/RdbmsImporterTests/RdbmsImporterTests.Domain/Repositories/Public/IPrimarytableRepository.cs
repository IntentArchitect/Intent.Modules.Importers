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
    public interface IPrimarytableRepository : IEFRepository<Primarytable, Primarytable>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int primarytableid, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Primarytable?> FindByIdAsync(int primarytableid, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Primarytable?> FindByIdAsync(int primarytableid, Func<IQueryable<Primarytable>, IQueryable<Primarytable>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Primarytable>> FindByIdsAsync(int[] primarytableids, CancellationToken cancellationToken = default);
    }
}