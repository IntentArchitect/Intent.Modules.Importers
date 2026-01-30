using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestDataGenerator.Application.Interfaces.Manual
{
    public interface IFullEntity : IAuditable, ISoftDeletable
    {
        Task<Dictionary<string, object>> GetMetadataAsync(CancellationToken cancellationToken = default);
        Task SetMetadataAsync(string key, object value, CancellationToken cancellationToken = default);
        Task<string> GetEntityTypeAsync(CancellationToken cancellationToken = default);
        Task CloneAsync(CancellationToken cancellationToken = default);
    }
}
