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
    public interface IParentRepository : IEFRepository<Parent, Parent>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>((Guid Id, Guid Id2) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Parent?> FindByIdAsync((Guid Id, Guid Id2) id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<Parent?> FindByIdAsync((Guid Id, Guid Id2) id, Func<IQueryable<Parent>, IQueryable<Parent>> queryOptions, CancellationToken cancellationToken = default);
    }
}