using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using RdbmsImporterTests.Domain.Entities.Public;
using RdbmsImporterTests.Domain.Repositories;
using RdbmsImporterTests.Domain.Repositories.Public;
using RdbmsImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.Public
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class LegacyTableRepository : RepositoryBase<LegacyTable, LegacyTable, PostgresAppDbContext>, ILegacyTableRepository
    {
        private readonly PostgresAppDbContext _dbContext;

        public LegacyTableRepository(PostgresAppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public void Add(LegacyTable entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO LegacyTables (LegacyId, LegacyColumn, BadDate) VALUES({entity.LegacyId}, {entity.LegacyColumn}, {entity.BadDate})");
        }
    }
}