using Intent.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.CSharp.Importer.Importer;

internal static class StereotypeExtensions
{
    public static IStereotypePersistable GetOrCreateStereotype(
        this IElementPersistable element,
        string stereotypeDefinitionId,
        string name, 
        string packageDefinitionId, 
        string packageDefinitionName,
        Action<IStereotypePersistable>? initAction = null)
    {
        var stereotype = element.Stereotypes.SingleOrDefault(x => x.DefinitionId == stereotypeDefinitionId);
        stereotype ??= element.Stereotypes.Add(stereotypeDefinitionId, name, packageDefinitionId, packageDefinitionName, initAction);

        return stereotype;
    }

    public static IStereotypePropertyPersistable GetOrCreateProperty(
        this IStereotypePersistable stereotype,
        string propertyId,
        string name,
        string value,
        Action<IStereotypePropertyPersistable>? initAction = null)
    {
        var property = stereotype.Properties.SingleOrDefault(p => p.DefinitionId == propertyId);
        property ??= stereotype.Properties.Add(propertyId, name, value, initAction);

        return property;
    }
}
