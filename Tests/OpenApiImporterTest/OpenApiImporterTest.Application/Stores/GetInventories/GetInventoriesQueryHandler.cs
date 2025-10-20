using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetInventories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetInventoriesQueryHandler : IRequestHandler<GetInventoriesQuery, Dictionary<string, int>>
    {
        [IntentManaged(Mode.Merge)]
        public GetInventoriesQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<Dictionary<string, int>> Handle(GetInventoriesQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetInventoriesQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}