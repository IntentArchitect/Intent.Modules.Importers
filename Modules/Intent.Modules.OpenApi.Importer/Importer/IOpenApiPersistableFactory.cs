using System.Collections.Generic;
using Intent.IArchitect.Agent.Persistence.Model;
using Microsoft.OpenApi.Models;

namespace Intent.Modules.OpenApi.Importer.Importer;

public interface IOpenApiPersistableFactory
{
    ElementPersistable GetIntentType(string openApiType);
    ResolvedType Resolve(OpenApiSchema schema, string? typeNameContext = null, bool impliedNullables = false, ISet<string>? requiredProperties = null);
}
