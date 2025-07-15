using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using TestDataGenerator.Domain.Contracts.RepoTest;
using TestDataGenerator.Domain.Entities.RepoTest;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace TestDataGenerator.Domain.Repositories.RepoTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IAggregateRoot1Repository : IEFRepository<AggregateRoot1, AggregateRoot1>
    {
        [IntentManaged(Mode.Fully)]
        Task<TProjection?> FindByIdProjectToAsync<TProjection>(Guid id, CancellationToken cancellationToken = default);
        void Operation_Params0_ReturnsV_Collection0();
        SpResult Operation_Params0_ReturnsD_Collection0();
        List<SpResult> Operation_Params0_ReturnsD_Collection1();
        AggregateRoot1 Operation_Params0_ReturnsE_Collection0();
        List<AggregateRoot1> Operation_Params0_ReturnsE_Collection1();
        Task Operation_Params0_ReturnsV_Collection0Async(CancellationToken cancellationToken = default);
        Task<SpResult> Operation_Params0_ReturnsD_Collection0Async(CancellationToken cancellationToken = default);
        Task<List<SpResult>> Operation_Params0_ReturnsD_Collection1Async(CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Operation_Params0_ReturnsE_Collection0Async(CancellationToken cancellationToken = default);
        Task<List<AggregateRoot1>> Operation_Params0_ReturnsE_Collection1Async(CancellationToken cancellationToken = default);
        void Operation_Params1_ReturnsV_Collection0(SpParameter param);
        SpResult Operation_Params1_ReturnsD_Collection0(SpParameter param);
        List<SpResult> Operation_Params1_ReturnsD_Collection1(SpParameter param);
        AggregateRoot1 Operation_Params1_ReturnsE_Collection0(SpParameter param);
        List<AggregateRoot1> Operation_Params1_ReturnsE_Collection1(SpParameter param);
        Task Operation_Params1_ReturnsV_Collection0Async(SpParameter param, CancellationToken cancellationToken = default);
        Task<SpResult> Operation_Params1_ReturnsD_Collection0Async(SpParameter param, CancellationToken cancellationToken = default);
        Task<List<SpResult>> Operation_Params1_ReturnsD_Collection1Async(SpParameter param, CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Operation_Params1_ReturnsE_Collection0Async(SpParameter param, CancellationToken cancellationToken = default);
        Task<List<AggregateRoot1>> Operation_Params1_ReturnsE_Collection1Async(SpParameter param, CancellationToken cancellationToken = default);
        void Operation_Params3_ReturnsV_Collection0(SpParameter param, AggregateRoot1 aggrParam, string strParam);
        SpResult Operation_Params3_ReturnsD_Collection0(SpParameter param, AggregateRoot1 aggrParam, string strParam);
        List<SpResult> Operation_Params3_ReturnsD_Collection1(SpParameter param, AggregateRoot1 aggrParam, string strParam);
        AggregateRoot1 Operation_Params3_ReturnsE_Collection0(SpParameter param, AggregateRoot1 aggrParam, string strParam);
        List<AggregateRoot1> Operation_Params3_ReturnsE_Collection1(SpParameter param, AggregateRoot1 aggrParam, string strParam);
        Task Operation_Params3_ReturnsV_Collection0Async(SpParameter param, AggregateRoot1 aggrParam, string strParam, CancellationToken cancellationToken = default);
        Task<SpResult> Operation_Params3_ReturnsD_Collection0Async(SpParameter param, AggregateRoot1 aggrParam, string strParam, CancellationToken cancellationToken = default);
        Task<List<SpResult>> Operation_Params3_ReturnsD_Collection1Async(SpParameter param, AggregateRoot1 aggrParam, string strParam, CancellationToken cancellationToken = default);
        Task<AggregateRoot1> Operation_Params3_ReturnsE_Collection0Async(SpParameter param, AggregateRoot1 aggrParam, string strParam, CancellationToken cancellationToken = default);
        Task<List<AggregateRoot1>> Operation_Params3_ReturnsE_Collection1Async(SpParameter param, AggregateRoot1 aggrParam, string strParam, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AggregateRoot1?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<AggregateRoot1?> FindByIdAsync(Guid id, Func<IQueryable<AggregateRoot1>, IQueryable<AggregateRoot1>> queryOptions, CancellationToken cancellationToken = default);
        [IntentManaged(Mode.Fully)]
        Task<List<AggregateRoot1>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default);
    }
}