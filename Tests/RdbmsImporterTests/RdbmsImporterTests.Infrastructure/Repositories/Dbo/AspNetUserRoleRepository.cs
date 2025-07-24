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
    public class AspNetUserRoleRepository : RepositoryBase<AspNetUserRole, AspNetUserRole, ApplicationDbContext>, IAspNetUserRoleRepository
    {
        public AspNetUserRoleRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            (string UserId, string RoleId) id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.UserId == id.UserId && x.RoleId == id.RoleId, cancellationToken);
        }

        public async Task<AspNetUserRole?> FindByIdAsync(
            (string UserId, string RoleId) id,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.UserId == id.UserId && x.RoleId == id.RoleId, cancellationToken);
        }

        public async Task<AspNetUserRole?> FindByIdAsync(
            (string UserId, string RoleId) id,
            Func<IQueryable<AspNetUserRole>, IQueryable<AspNetUserRole>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.UserId == id.UserId && x.RoleId == id.RoleId, queryOptions, cancellationToken);
        }
    }
}