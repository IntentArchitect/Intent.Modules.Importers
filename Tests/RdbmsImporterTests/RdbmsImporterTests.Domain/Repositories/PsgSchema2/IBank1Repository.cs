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
    public interface IBank1Repository : IEFRepository<Bank1, Bank1>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Bank1?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Bank1?> FindByIdAsync(Guid id, Func<IQueryable<Bank1>, IQueryable<Bank1>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<Bank1>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}