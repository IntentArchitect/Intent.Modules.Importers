using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model.Common;

namespace Intent.Modules.Rdbms.Importer.Tasks.Mappers;

public static class ModuleHelper
{
    /// <summary>
    /// Apply the necessary package references.
    /// </summary>
    /// <remarks>
    /// Assumes PackageModelPersistable.Save() will ensure distinct reference persistence.
    /// </remarks>
    public static void ApplyRelevantReferences(PackageModelPersistable package, IApplicationConfigurationProvider configurationProvider)
    {
        var moduleIds = configurationProvider.GetInstalledModules().Select(x => x.ModuleId).ToHashSet();

        if (moduleIds.Contains(Constants.Packages.CommonTypes.ModuleName))
        {
            package.References.Add(new PackageReferenceModel
            {
                PackageId = Constants.Packages.CommonTypes.DefinitionPackageId,
                Name = Constants.Packages.CommonTypes.DefinitionPackageName,
                Module = Constants.Packages.CommonTypes.ModuleName,
                IsExternal = true
            });
        }

        if (moduleIds.Contains(Constants.Packages.Rdbms.ModuleName))
        {
            package.References.Add(new PackageReferenceModel
            {
                PackageId = Constants.Packages.Rdbms.DefinitionPackageId,
                Name = Constants.Packages.Rdbms.DefinitionPackageName,
                Module = Constants.Packages.Rdbms.ModuleName,
                IsExternal = true
            });
        }

        if (moduleIds.Contains(Constants.Packages.EntityFrameworkCore.ModuleName))
        {
            package.References.Add(new PackageReferenceModel
            {
                PackageId = Constants.Packages.EntityFrameworkCore.DefinitionPackageId,
                Name = Constants.Packages.EntityFrameworkCore.DefinitionPackageName,
                Module = Constants.Packages.EntityFrameworkCore.ModuleName,
                IsExternal = true
            });
        }

        if (moduleIds.Contains(Constants.Packages.EntityFrameworkCoreRepository.ModuleName))
        {
            package.References.Add(new PackageReferenceModel
            {
                PackageId = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageId,
                Name = Constants.Packages.EntityFrameworkCoreRepository.DefinitionPackageName,
                Module = Constants.Packages.EntityFrameworkCoreRepository.ModuleName,
                IsExternal = true
            });
        }
    }
}