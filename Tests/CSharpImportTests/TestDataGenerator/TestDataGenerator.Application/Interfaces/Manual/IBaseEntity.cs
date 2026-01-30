using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestDataGenerator.Application.Interfaces.Manual
{
    public interface IBaseEntity
    {
        Task<bool> ValidateAsync(CancellationToken cancellationToken = default);
        Task<string> SerializeAsync(CancellationToken cancellationToken = default);
        Task DeserializeAsync(string data, CancellationToken cancellationToken = default);
    }
}
