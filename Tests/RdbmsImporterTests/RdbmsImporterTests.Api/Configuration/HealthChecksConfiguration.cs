using HealthChecks.UI.Client;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.HealthChecks.HealthChecksConfiguration", Version = "1.0")]

namespace RdbmsImporterTests.Api.Configuration
{
    public static class HealthChecksConfiguration
    {
        public static IServiceCollection ConfigureHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();
            hcBuilder.AddSqlServer(configuration.GetConnectionString("DefaultConnection")!, name: "DefaultConnection", tags: new[] { "database", "SqlServer" });
            hcBuilder.AddNpgSql(configuration.GetConnectionString("PostgresApp")!, name: "PostgresApp", tags: new[] { "database", "PostgreSql" });

            return services;
        }

        public static IEndpointRouteBuilder MapDefaultHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/hc", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
            return endpoints;
        }
    }
}