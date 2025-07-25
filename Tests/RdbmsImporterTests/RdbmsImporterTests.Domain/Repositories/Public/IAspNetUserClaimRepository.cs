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
    public interface IAspNetUserClaimRepository : IEFRepository<AspNetUserClaim, AspNetUserClaim>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserClaim?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserClaim?> FindByIdAsync(int id, Func<IQueryable<AspNetUserClaim>, IQueryable<AspNetUserClaim>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AspNetUserClaim>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default);
    }
}