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
    public interface IFKTableRepository : IEFRepository<FKTable, FKTable>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int fKTableId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<FKTable?> FindByIdAsync(int fKTableId, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<FKTable?> FindByIdAsync(int fKTableId, Func<IQueryable<FKTable>, IQueryable<FKTable>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<FKTable>> FindByIdsAsync(int[] fKTableIds, CancellationToken cancellationToken = default);
    }
}