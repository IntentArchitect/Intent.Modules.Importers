using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.MetadataSynchronizer.OpenApi.CLI.ServiceCreation
{
    public interface IServiceCreationStrategy
    {
        Persistables CreateServices(IList<AbstractServiceOperationModel> serviceDefinitions);
        Persistables GetDomainPersistables();
    }
}
