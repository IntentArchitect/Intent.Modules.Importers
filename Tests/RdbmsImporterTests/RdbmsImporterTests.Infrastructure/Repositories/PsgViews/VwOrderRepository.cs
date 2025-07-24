using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using RdbmsImporterTests.Domain.Entities.PsgViews;
using RdbmsImporterTests.Domain.Repositories;
using RdbmsImporterTests.Domain.Repositories.PsgViews;
using RdbmsImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace RdbmsImporterTests.Infrastructure.Repositories.PsgViews
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class VwOrderRepository : RepositoryBase<VwOrder, VwOrder, PostgresAppDbContext>, IVwOrderRepository
    {
        private readonly PostgresAppDbContext _dbContext;

        public VwOrderRepository(PostgresAppDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public void Add(VwOrder entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO VwOrders (Id, CustomerId, OrderDate, RefNo) VALUES({entity.Id}, {entity.CustomerId}, {entity.OrderDate}, {entity.RefNo})");
        }
    }
}