using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using RdbmsImporterTests.Domain.Entities.Dbo;
using RdbmsImporterTests.Domain.Repositories;
using RdbmsImporterTests.Domain.Repositories.Dbo;
using RdbmsImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.Dbo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class LegacyTableRepository : RepositoryBase<LegacyTable, LegacyTable, ApplicationDbContext>, ILegacyTableRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public LegacyTableRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public void Add(LegacyTable entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO LegacyTables (LegacyId, LegacyColumn, BadDate) VALUES({entity.LegacyId}, {entity.LegacyColumn}, {entity.BadDate})");
        }
    }
}