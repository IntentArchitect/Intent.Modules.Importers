using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Entities.Public;
using RdbmsImporterTests.Domain.Repositories;
using RdbmsImporterTests.Domain.Repositories.Public;
using RdbmsImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.Public
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ParentRepository : RepositoryBase<Parent, Parent, PostgresAppDbContext>, IParentRepository
    {
        public ParentRepository(PostgresAppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            (Guid Id, Guid Id2) id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id.Id && x.Id2 == id.Id2, cancellationToken);
        }

        public async Task<Parent?> FindByIdAsync((Guid Id, Guid Id2) id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id.Id && x.Id2 == id.Id2, cancellationToken);
        }

        public async Task<Parent?> FindByIdAsync(
            (Guid Id, Guid Id2) id,
            Func<IQueryable<Parent>, IQueryable<Parent>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id.Id && x.Id2 == id.Id2, queryOptions, cancellationToken);
        }
    }
}