using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;
using OpenApiImporterTest.Application.Common.Security;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetInventories
{
    [Authorize]
    public class GetInventoriesQuery : IRequest<Dictionary<string, int>>, IQuery
    {
        public GetInventoriesQuery()
        {
        }
    }
}