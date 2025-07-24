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
    public class AspNetUserClaimRepository : RepositoryBase<AspNetUserClaim, AspNetUserClaim, PostgresAppDbContext>, IAspNetUserClaimRepository
    {
        public AspNetUserClaimRepository(PostgresAppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<AspNetUserClaim?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<AspNetUserClaim?> FindByIdAsync(
            int id,
            Func<IQueryable<AspNetUserClaim>, IQueryable<AspNetUserClaim>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<AspNetUserClaim>> FindByIdsAsync(int[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}