using System.Collections.Generic;
using Intent.MetadataSynchronizer;

namespace Intent.Modules.OpenApi.Importer.Importer.ServiceCreation;

public interface IServiceCreationStrategy
{
    Persistables CreateServices(IList<AbstractServiceOperationModel> serviceDefinitions);
}
