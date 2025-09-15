using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Entities.Dbo;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace RdbmsImporterTests.Domain.Repositories.Dbo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IPrimaryTableRepository : IEFRepository<PrimaryTable, PrimaryTable>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int primaryTableId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PrimaryTable?> FindByIdAsync(int primaryTableId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<PrimaryTable?> FindByIdAsync(int primaryTableId, Func<IQueryable<PrimaryTable>, IQueryable<PrimaryTable>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<PrimaryTable>> FindByIdsAsync(int[] primaryTableIds, CancellationToken cancellationToken = default);
    }
}