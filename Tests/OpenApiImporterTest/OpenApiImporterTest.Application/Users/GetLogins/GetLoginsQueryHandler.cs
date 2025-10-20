using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetLogins
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetLoginsQueryHandler : IRequestHandler<GetLoginsQuery, string>
    {
        [IntentManaged(Mode.Merge)]
        public GetLoginsQueryHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<string> Handle(GetLoginsQuery request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (GetLoginsQueryHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}