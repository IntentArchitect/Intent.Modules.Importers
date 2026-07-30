using System;
using Intent.Engine;
using Intent.Modules.Rdbms.Importer.Tasks.Helpers;
using Intent.Modules.Rdbms.Importer.Tasks.Models;
using Intent.Utils;

namespace Intent.Modules.Rdbms.Importer.Tasks;

public class DatabaseImportSettingsResolution : ModuleTaskBase<DatabaseImportSettingsResolutionInput>
{
    private readonly IMetadataManager _metadataManager;

    public DatabaseImportSettingsResolution(IMetadataManager metadataManager)
    {
        _metadataManager = metadataManager;
    }

    public override string TaskTypeId => "Intent.Modules.Rdbms.Importer.Tasks.DatabaseImportSettingsResolution";
    public override string TaskTypeName => "Database Import Settings Resolution";
    public override int Order => 0;

    protected override ValidationResult ValidateInputModel(DatabaseImportSettingsResolutionInput inputModel)
    {
        if (string.IsNullOrWhiteSpace(inputModel.ApplicationId))
        {
            return ValidationResult.ErrorResult("Application Id is required.");
        }

        if (string.IsNullOrWhiteSpace(inputModel.PackageId))
        {
            return ValidationResult.ErrorResult("Package Id is required.");
        }

        return ValidationResult.SuccessResult();
    }

    protected override ExecuteResult ExecuteModuleTask(DatabaseImportSettingsResolutionInput inputModel)
    {
        if (!_metadataManager.TryGetApplicationPackage(inputModel.ApplicationId, inputModel.PackageId, out var package, out var errorMessage))
        {
            throw new Exception(errorMessage);
        }

        var resolution = SettingsHelper.ResolveDatabaseImportSettings(package.FileLocation);
        var executionResult = new ExecuteResult();
        executionResult.Result = new DatabaseImportSettingsResolutionResult
        {
            EntityNameConvention = resolution.Settings.EntityNameConvention,
            AttributeNameConvention = resolution.Settings.AttributeNameConvention,
            TableStereotypes = resolution.Settings.TableStereotypes,
            TypesToExport = string.Join(";", resolution.Settings.TypesToExport),
            ImportFilterFilePath = resolution.Settings.ImportFilterFilePath,
            ConnectionString = resolution.Settings.ConnectionString,
            StoredProcedureType = resolution.Settings.StoredProcedureType,
            SettingPersistence = resolution.Settings.SettingPersistence.ToString(),
            DatabaseType = resolution.Settings.DatabaseType,
            FilterType = resolution.Settings.FilterType,
            AllowDeletions = resolution.Settings.AllowDeletions,
            PreserveAttributeTypes = resolution.Settings.PreserveAttributeTypes,
            Source = resolution.Source.ToString(),
            UserLocalSettingsPath = SettingsHelper.GetUserLocalSettingsPath(package.FileLocation)
        };

        return executionResult;
    }
}

