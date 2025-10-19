using Intent.MetadataSynchronizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.OpenApi.Importer.Importer.ServiceCreation
{
    public interface IServiceCreationStrategy
    {
        Persistables CreateServices(IList<AbstractServiceOperationModel> serviceDefinitions);
        Persistables GetDomainPersistables();
    }
}
