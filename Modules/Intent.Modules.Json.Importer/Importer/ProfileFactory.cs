using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Json.Importer.Importer.Visitors;

namespace Intent.Modules.Json.Importer.Importer;

public enum ImportProfile
{
    [Description("Domain Document DB Profile")]
    DomainDocumentDB = 1,
    [Description("Eventing Messages Profile")]
    EventingMessages = 2,
    [Description("Services DTOs Profile")]
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

    public static ImportProfile[] GetAvailableProfilesForPackageSpecialization(string? packageSpecialization)
    {
        var profiles = new List<ImportProfile>();

        if (packageSpecialization == "Domain Package")
        {
            profiles.Add(ImportProfile.DomainDocumentDB);
        }

        if (packageSpecialization == "Eventing Package")
        {
            profiles.Add(ImportProfile.EventingMessages);
        }

        if (packageSpecialization == "Services Package")
        {
            profiles.Add(ImportProfile.ServicesDtos);
        }

        return profiles.ToArray();
    }
}
