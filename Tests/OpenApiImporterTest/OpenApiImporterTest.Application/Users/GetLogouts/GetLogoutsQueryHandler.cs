using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetLogouts
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetLogoutsQueryHandler : IRequestHandler<GetLogoutsQuery, int>
    {
        [IntentManaged(Mode.Merge)]
        public GetLogoutsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<int> Handle(GetLogoutsQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetLogoutsQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}