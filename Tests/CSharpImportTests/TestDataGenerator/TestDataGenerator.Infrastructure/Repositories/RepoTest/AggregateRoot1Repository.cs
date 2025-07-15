using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Contracts.RepoTest;
using TestDataGenerator.Domain.Entities.RepoTest;
using TestDataGenerator.Domain.Repositories;
using TestDataGenerator.Domain.Repositories.RepoTest;
using TestDataGenerator.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace TestDataGenerator.Infrastructure.Repositories.RepoTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class AggregateRoot1Repository : RepositoryBase<AggregateRoot1, AggregateRoot1, ApplicationDbContext>, IAggregateRoot1Repository
    {
        public AggregateRoot1Repository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void Operation_Params0_ReturnsV_Collection0()
        {
            // TODO: Implement Operation_Params0_ReturnsV_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SpResult Operation_Params0_ReturnsD_Collection0()
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<SpResult> Operation_Params0_ReturnsD_Collection1()
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection1 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AggregateRoot1 Operation_Params0_ReturnsE_Collection0()
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<AggregateRoot1> Operation_Params0_ReturnsE_Collection1()
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection1 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation_Params0_ReturnsV_Collection0Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsV_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<SpResult> Operation_Params0_ReturnsD_Collection0Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<SpResult>> Operation_Params0_ReturnsD_Collection1Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsD_Collection1Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<AggregateRoot1> Operation_Params0_ReturnsE_Collection0Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<AggregateRoot1>> Operation_Params0_ReturnsE_Collection1Async(CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params0_ReturnsE_Collection1Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void Operation_Params1_ReturnsV_Collection0(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsV_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SpResult Operation_Params1_ReturnsD_Collection0(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<SpResult> Operation_Params1_ReturnsD_Collection1(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection1 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AggregateRoot1 Operation_Params1_ReturnsE_Collection0(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<AggregateRoot1> Operation_Params1_ReturnsE_Collection1(SpParameter param)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection1 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation_Params1_ReturnsV_Collection0Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsV_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<SpResult> Operation_Params1_ReturnsD_Collection0Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<SpResult>> Operation_Params1_ReturnsD_Collection1Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsD_Collection1Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<AggregateRoot1> Operation_Params1_ReturnsE_Collection0Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<AggregateRoot1>> Operation_Params1_ReturnsE_Collection1Async(
            SpParameter param,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params1_ReturnsE_Collection1Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public void Operation_Params3_ReturnsV_Collection0(SpParameter param, AggregateRoot1 aggrParam, string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsV_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public SpResult Operation_Params3_ReturnsD_Collection0(SpParameter param, AggregateRoot1 aggrParam, string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<SpResult> Operation_Params3_ReturnsD_Collection1(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection1 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AggregateRoot1 Operation_Params3_ReturnsE_Collection0(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection0 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public List<AggregateRoot1> Operation_Params3_ReturnsE_Collection1(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection1 (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Operation_Params3_ReturnsV_Collection0Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsV_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<SpResult> Operation_Params3_ReturnsD_Collection0Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<SpResult>> Operation_Params3_ReturnsD_Collection1Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsD_Collection1Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<AggregateRoot1> Operation_Params3_ReturnsE_Collection0Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection0Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<AggregateRoot1>> Operation_Params3_ReturnsE_Collection1Async(
            SpParameter param,
            AggregateRoot1 aggrParam,
            string strParam,
            CancellationToken cancellationToken = default)
        {
            // TODO: Implement Operation_Params3_ReturnsE_Collection1Async (AggregateRoot1Repository) functionality
            throw new NotImplementedException("Your implementation here...");
        }

        public async Task<TProjection?> FindByIdProjectToAsync<TProjection>(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await FindProjectToAsync<TProjection>(x => x.Id == id, cancellationToken);
        }

        public async Task<AggregateRoot1?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<AggregateRoot1?> FindByIdAsync(
            Guid id,
            Func<IQueryable<AggregateRoot1>, IQueryable<AggregateRoot1>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, queryOptions, cancellationToken);
        }

        public async Task<List<AggregateRoot1>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            // Force materialization - Some combinations of .net9 runtime and EF runtime crash with "Convert ReadOnlySpan to List since expression trees can't handle ref struct"
            var idList = ids.ToList();
            return await FindAllAsync(x => idList.Contains(x.Id), cancellationToken);
        }
    }
}