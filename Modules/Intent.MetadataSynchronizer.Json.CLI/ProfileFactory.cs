using Intent.MetadataSynchronizer.Json.CLI.Visitors;
using Intent.Modules.Common.Templates;
using Intent.Utils;

namespace Intent.MetadataSynchronizer.Json.CLI;

public enum ImportProfile
{
    DomainDocumentDB = 1,
    EventingMessages = 2,
    ServicesDtos = 3
}

public static class ProfileFactory
{
    public static IJsonElementVisitor GetVisitorForProfile(ImportProfile profile)
    {
        return profile switch
        {
            ImportProfile.DomainDocumentDB => new DocumentDomainVisitor(),
            ImportProfile.EventingMessages => new EventingMessagesVisitor(),
            ImportProfile.ServicesDtos => new ServicesDtosVisitor(),
            _ => throw new ArgumentOutOfRangeException($"Profile '{profile}' is not supported.")
        };
    }
}