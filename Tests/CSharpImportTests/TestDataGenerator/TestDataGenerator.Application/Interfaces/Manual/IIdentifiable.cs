using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestDataGenerator.Application.Interfaces.Manual
{
    public interface IIdentifiable : IBaseEntity
    {
        Task<Guid> GetIdAsync(CancellationToken cancellationToken = default);
        Task SetIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(CancellationToken cancellationToken = default);
    }
}
