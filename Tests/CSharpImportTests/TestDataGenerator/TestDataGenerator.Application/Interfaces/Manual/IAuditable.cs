using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestDataGenerator.Application.Interfaces.Manual
{
    public interface IAuditable : IIdentifiable
    {
        Task<DateTime> GetCreatedDateAsync(CancellationToken cancellationToken = default);
        Task<string> GetCreatedByAsync(CancellationToken cancellationToken = default);
        Task<DateTime?> GetModifiedDateAsync(CancellationToken cancellationToken = default);
        Task<string> GetModifiedByAsync(CancellationToken cancellationToken = default);
        Task UpdateAuditInfoAsync(string modifiedBy, CancellationToken cancellationToken = default);
    }
}
