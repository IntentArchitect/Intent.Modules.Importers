using Intent.IArchitect.Agent.Persistence.Model;
using Microsoft.OpenApi.Models;

namespace Intent.MetadataSynchronizer.OpenApi.CLI
{
    public interface IOpenApiPresistableFactory
    {
        ElementPersistable GetIntentType(string openApiType);
        ResolvedType Resolve(OpenApiSchema schema, string typeNameContext = null, bool impliedNullables = false, ISet<string> requiredProperties = null);
    }
}