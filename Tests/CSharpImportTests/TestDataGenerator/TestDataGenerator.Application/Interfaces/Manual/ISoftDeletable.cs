using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestDataGenerator.Application.Interfaces.Manual
{
    public interface ISoftDeletable : IIdentifiable
    {
        Task<bool> IsDeletedAsync(CancellationToken cancellationToken = default);
        Task<DateTime?> GetDeletedDateAsync(CancellationToken cancellationToken = default);
        Task<string> GetDeletedByAsync(CancellationToken cancellationToken = default);
        Task SoftDeleteAsync(string deletedBy, CancellationToken cancellationToken = default);
        Task RestoreAsync(CancellationToken cancellationToken = default);
    }
}
