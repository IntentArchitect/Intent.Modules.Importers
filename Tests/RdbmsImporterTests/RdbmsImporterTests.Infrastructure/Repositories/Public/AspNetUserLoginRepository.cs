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
    public class AspNetUserLoginRepository : RepositoryBase<AspNetUserLogin, AspNetUserLogin, PostgresAppDbContext>, IAspNetUserLoginRepository
    {
        public AspNetUserLoginRepository(PostgresAppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            (string LoginProvider, string ProviderKey) id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.LoginProvider == id.LoginProvider && x.ProviderKey == id.ProviderKey, cancellationToken);
        }

        public async Task<AspNetUserLogin?> FindByIdAsync(
            (string LoginProvider, string ProviderKey) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.LoginProvider == id.LoginProvider && x.ProviderKey == id.ProviderKey, cancellationToken);
        }

        public async Task<AspNetUserLogin?> FindByIdAsync(
            (string LoginProvider, string ProviderKey) id,
            Func<IQueryable<AspNetUserLogin>, IQueryable<AspNetUserLogin>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.LoginProvider == id.LoginProvider && x.ProviderKey == id.ProviderKey, queryOptions, cancellationToken);
        }
    }
}