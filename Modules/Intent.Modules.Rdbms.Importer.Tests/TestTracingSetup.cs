using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Intent.Engine;
using Intent.IArchitect.Common.Publishing;
using Intent.Utils;

namespace Intent.Modules.Rdbms.Importer.Tests;

/// <summary>
/// SettingsHelper.PersistSettings logs via Logging.Log and PackageModelPersistable.Save publishes via
/// DomainPublisher.Instance - both wired up by the host application at startup and both null outside
/// it, so any test exercising those code paths needs stand-ins installed once for the whole run.
/// </summary>
internal static class TestTracingSetup
{
    [ModuleInitializer]
    public static void Initialize()
    {
        Logging.SetTracing(new NoOpTracing());
        DomainPublisher.Set(new NoOpDomainEventDispatcher());
    }

    private sealed class NoOpTracing : ITracing
    {
        public void Debug(string message)
        {
        }

        public void Failure(Exception exception)
        {
        }

        public void Failure(string exceptionMessage)
        {
        }

        public void Info(string message)
        {
        }

        public void Warning(string message)
        {
        }
    }

    private sealed class NoOpDomainEventDispatcher : IDomainEventDispatcher
    {
        public Task<TResponse> Request<TResponse>(IDomainRequest<TResponse> request) => Task.FromResult<TResponse>(default!);

        public Task Publish(IDomainEvent @event) => Task.CompletedTask;
    }
}
