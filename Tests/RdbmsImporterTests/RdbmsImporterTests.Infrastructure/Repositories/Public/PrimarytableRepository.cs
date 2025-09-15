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
    public class PrimarytableRepository : RepositoryBase<Primarytable, Primarytable, PostgresAppDbContext>, IPrimarytableRepository
    {
        public PrimarytableRepository(PostgresAppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int primarytableid,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Primarytableid == primarytableid, cancellationToken);
        }

        public async Task<Primarytable?> FindByIdAsync(int primarytableid, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Primarytableid == primarytableid, cancellationToken);
        }

        public async Task<Primarytable?> FindByIdAsync(
            int primarytableid,
            Func<IQueryable<Primarytable>, IQueryable<Primarytable>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Primarytableid == primarytableid, queryOptions, cancellationToken);
        }

        public async Task<List<Primarytable>> FindByIdsAsync(
            int[] primarytableids,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = primarytableids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Primarytableid), cancellationToken);
        }
    }
}