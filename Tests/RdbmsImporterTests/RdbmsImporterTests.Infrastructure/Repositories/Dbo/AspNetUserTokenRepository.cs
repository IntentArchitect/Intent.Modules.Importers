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
    public class AspNetUserTokenRepository : RepositoryBase<AspNetUserToken, AspNetUserToken, ApplicationDbContext>, IAspNetUserTokenRepository
    {
        public AspNetUserTokenRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            (string UserId, string LoginProvider, string Name) id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.UserId == id.UserId && x.LoginProvider == id.LoginProvider && x.Name == id.Name, cancellationToken);
        }

        public async Task<AspNetUserToken?> FindByIdAsync(
            (string UserId, string LoginProvider, string Name) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.UserId == id.UserId && x.LoginProvider == id.LoginProvider && x.Name == id.Name, cancellationToken);
        }

        public async Task<AspNetUserToken?> FindByIdAsync(
            (string UserId, string LoginProvider, string Name) id,
            Func<IQueryable<AspNetUserToken>, IQueryable<AspNetUserToken>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.UserId == id.UserId && x.LoginProvider == id.LoginProvider && x.Name == id.Name, queryOptions, cancellationToken);
        }
    }
}