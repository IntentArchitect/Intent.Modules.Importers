using System;
using Intent.Modules.Json.Importer.Importer.Visitors;

namespace Intent.Modules.Json.Importer.Importer;

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
