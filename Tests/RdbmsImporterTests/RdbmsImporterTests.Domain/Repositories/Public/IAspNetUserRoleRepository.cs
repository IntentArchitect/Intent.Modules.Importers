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
    public interface IAspNetUserRoleRepository : IEFRepository<AspNetUserRole, AspNetUserRole>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>((string UserId, string RoleId) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserRole?> FindByIdAsync((string UserId, string RoleId) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AspNetUserRole?> FindByIdAsync((string UserId, string RoleId) id, Func<IQueryable<AspNetUserRole>, IQueryable<AspNetUserRole>> queryOptions, CancellationToken cancellationToken = default);
    }
}