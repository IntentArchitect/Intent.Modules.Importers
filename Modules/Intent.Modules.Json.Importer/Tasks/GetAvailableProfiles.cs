using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Intent.Engine;
using Intent.Modules.Json.Importer.Importer;
using Intent.Modules.Json.Importer.Tasks.Helpers;
using Intent.Modules.Json.Importer.Tasks.Models;

namespace Intent.Modules.Json.Importer.Tasks;

public class GetAvailableProfiles : ModuleTaskBase<GetAvailableProfilesInputModel>
{
    private readonly IMetadataManager _metadataManager;

    public GetAvailableProfiles(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.Json.Importer.Tasks.GetAvailableProfiles";
    public override string TaskTypeName => "Get Available Profiles";

    protected override ValidationResult ValidateInputModel(GetAvailableProfilesInputModel inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.PackageId))
            return ValidationResult.ErrorResult("Package ID is required.");

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(GetAvailableProfilesInputModel inputModel)
    {
        var executionResult = new ExecuteResult();

        // Get available profiles from ProfileFactory
        var availableProfiles = ProfileFactory.GetAvailableProfilesForPackageSpecialization(inputModel.PackageSpecialization);
        
        // Convert to output format
        var profiles = availableProfiles
            .Select(p => new { id = p.ToString(), description = GetProfileDescription(p) })
            .ToArray();
        
        executionResult.Result = profiles;

        return executionResult;
    }

    private static string GetProfileDescription(ImportProfile profile)
    {
        var fieldInfo = profile.GetType().GetField(profile.ToString());
        var descriptionAttribute = fieldInfo?.GetCustomAttribute<DescriptionAttribute>();
        return descriptionAttribute?.Description ?? profile.ToString();
    }
}