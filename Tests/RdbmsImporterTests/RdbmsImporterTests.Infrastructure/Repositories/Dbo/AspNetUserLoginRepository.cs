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
    public class AspNetUserLoginRepository : RepositoryBase<AspNetUserLogin, AspNetUserLogin, ApplicationDbContext>, IAspNetUserLoginRepository
    {
        public AspNetUserLoginRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
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