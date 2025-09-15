using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using RdbmsImporterTests.Domain.Entities.Dbo;
using RdbmsImporterTests.Domain.Repositories;
using RdbmsImporterTests.Domain.Repositories.Dbo;
using RdbmsImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.Dbo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PrimaryTableRepository : RepositoryBase<PrimaryTable, PrimaryTable, ApplicationDbContext>, IPrimaryTableRepository
    {
        public PrimaryTableRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            int primaryTableId,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.PrimaryTableId == primaryTableId, cancellationToken);
        }

        public async Task<PrimaryTable?> FindByIdAsync(int primaryTableId, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PrimaryTableId == primaryTableId, cancellationToken);
        }

        public async Task<PrimaryTable?> FindByIdAsync(
            int primaryTableId,
            Func<IQueryable<PrimaryTable>, IQueryable<PrimaryTable>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.PrimaryTableId == primaryTableId, queryOptions, cancellationToken);
        }

        public async Task<List<PrimaryTable>> FindByIdsAsync(
            int[] primaryTableIds,
            CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = primaryTableIds.ToList();
            return await FindAllAsync(x => idList.Contains(x.PrimaryTableId), cancellationToken);
        }
    }
}