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
    public interface IAspNetUserTokenRepository : IEFRepository<AspNetUserToken, AspNetUserToken>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>((string UserId, string LoginProvider, string Name) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserToken?> FindByIdAsync((string UserId, string LoginProvider, string Name) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserToken?> FindByIdAsync((string UserId, string LoginProvider, string Name) id, Func<IQueryable<AspNetUserToken>, IQueryable<AspNetUserToken>> queryOptions, CancellationToken cancellationToken = default);
    }
}