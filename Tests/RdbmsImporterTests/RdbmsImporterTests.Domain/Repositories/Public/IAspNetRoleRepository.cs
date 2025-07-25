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
    public interface IAspNetRoleRepository : IEFRepository<AspNetRole, AspNetRole>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetRole?> FindByIdAsync(string id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetRole?> FindByIdAsync(string id, Func<IQueryable<AspNetRole>, IQueryable<AspNetRole>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AspNetRole>> FindByIdsAsync(string[] ids, CancellationToken cancellationToken = default);
    }
}