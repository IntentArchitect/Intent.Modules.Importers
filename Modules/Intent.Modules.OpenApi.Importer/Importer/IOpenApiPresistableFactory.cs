using Intent.IArchitect.Agent.Persistence.Model;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Intent.Modules.OpenApi.Importer.Importer
{
    public interface IOpenApiPresistableFactory
    {
        ElementPersistable GetIntentType(string openApiType);
        ResolvedType Resolve(OpenApiSchema schema, string? typeNameContext = null, bool impliedNullables = false, ISet<string>? requiredProperties = null);
    }
}
