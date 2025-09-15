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
    public class FktableRepository : RepositoryBase<Fktable, Fktable, PostgresAppDbContext>, IFktableRepository
    {
        public FktableRepository(PostgresAppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int fktableid,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Fktableid == fktableid, cancellationToken);
        }

        public async Task<Fktable?> FindByIdAsync(int fktableid, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Fktableid == fktableid, cancellationToken);
        }

        public async Task<Fktable?> FindByIdAsync(
            int fktableid,
            Func<IQueryable<Fktable>, IQueryable<Fktable>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Fktableid == fktableid, queryOptions, cancellationToken);
        }

        public async Task<List<Fktable>> FindByIdsAsync(int[] fktableids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = fktableids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Fktableid), cancellationToken);
        }
    }
}